using Aruba.Document.Api.Documents.Requests;
using Aruba.Document.Api.Documents.Responses;
using Aruba.Document.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace Aruba.Document.FunctionalTests.Documents;

public class DocumentsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IMongoDatabase _database;

    public DocumentsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    }

    [Fact]
    public async Task DocumentsController_Insert_ShouldCreateDocument_WhenValid()
    {
        // Given
        var payload = new InsertDocumentRequest
        {
            Type = DocumentType.Quote,
            CustomerName = "Mario Rossi",
            Description = "Test description"
        };

        // When
        var response = await _client.PostAsJsonAsync("/documents", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Mario Rossi");
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Insert_ShouldReturnBadRequest_WhenCustomerNameIsNull()
    {
        // Given
        var payload = new InsertDocumentRequest
        {
            Type = DocumentType.Quote,
            CustomerName = null,
            Description = "desc"
        };

        // When
        var response = await _client.PostAsJsonAsync("/documents", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DocumentsController_Insert_ShouldReturnBadRequest_WhenDescriptionIsNull()
    {
        // Given
        var payload = new InsertDocumentRequest
        {
            Type = DocumentType.Quote,
            CustomerName = "Mario",
            Description = null
        };

        // When
        var response = await _client.PostAsJsonAsync("/documents", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DocumentsController_GetById_ShouldReturnDocument_WhenExists()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync($"/documents/{document.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Id.Should().Be(document.Id);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_GetById_ShouldReturnNotFound_WhenNotExists()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.GetAsync($"/documents/{nonExistingId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_GetAll_ShouldReturnDocuments()
    {
        // Given
        await TestDataSeeder.InsertDocumentAsync(_database, new Domain.Models.Document(DocumentType.Quote, "Mario", "desc"));

        // When
        var response = await _client.GetAsync("/documents/all");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().NotBeEmpty();
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Update_ShouldUpdateDocument_WhenValid()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        var payload = new UpdateDocumentRequest
        {
            CustomerName = "Updated Name",
            Description = "Updated Description"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/documents/{document.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.CustomerName.Should().Be("Updated Name");
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Update_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();
        var payload = new UpdateDocumentRequest
        {
            CustomerName = "Updated",
            Description = "Updated"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/documents/{nonExistingId}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DocumentsController_Update_ShouldReturnBadRequest_WhenCustomerNameIsNullOrEmpty(string customerName)
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);
        var payload = new UpdateDocumentRequest
        {
            CustomerName = customerName,
            Description = "Updated"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/documents/{document.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DocumentsController_Update_ShouldReturnBadRequest_WhenDescriptionIsNullOrEmpty(string description)
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);
        var payload = new UpdateDocumentRequest
        {
            CustomerName = "Updated",
            Description = description
        };

        // When
        var response = await _client.PutAsJsonAsync($"/documents/{document.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnDocuments_WhenSearchingByDescription()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "Special description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=description&value=Special description");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);

        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnDocuments_WhenSearchingById()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync($"/documents/search?field=id&value={document.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnDocuments_WhenSearchingByType()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Proforma, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=type&value=Proforma");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnDocuments_WhenSearchingByStatus()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=status&value=Completed");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnEmptyList_WhenSearchingByInvalidStatus()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=status&value=Banana");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().BeEmpty();
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnEmptyList_WhenSearchingByInvalidType()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=type&value=InvalidType");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().BeEmpty();
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldReturnEmptyList_WhenFieldIsUnknown()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=unknown&value=test");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().BeEmpty();
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldWorkCaseInsensitive_OnFieldName()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=CuStOmErNaMe&value=Mario");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Search_ShouldWorkCaseInsensitive_OnEnumValues()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.SalesOrder, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.GetAsync("/documents/search?field=type&value=sAlEsOrDeR");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<DocumentResponse>>();
        result.Should().HaveCount(1);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnProforma_WhenSourceIsQuote()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=Proforma", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Should().NotBeNull();
        result.Type.Should().Be(DocumentType.Proforma);
        result.CustomerName.Should().Be("Mario Rossi");
        result.Description.Should().Be("Test description");
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnSalesOrder_WhenSourceIsProforma()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Proforma, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=SalesOrder", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Should().NotBeNull();
        result.Type.Should().Be(DocumentType.SalesOrder);
        result.CustomerName.Should().Be("Mario Rossi");
        result.Description.Should().Be("Test description");
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldLinkGeneratedDocumentToSource()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=Proforma", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.LinkedDocuments.Should().ContainSingle(id => id == document.Id);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnBadRequest_WhenQuoteGeneratesSalesOrder()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=SalesOrder", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnBadRequest_WhenProformaGeneratesQuote()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Proforma, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=Quote", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnBadRequest_WhenSalesOrderGeneratesAnyDocument()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.SalesOrder, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=Proforma", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/generate?type=Proforma", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_Generate_ShouldReturnGeneratedDocument_WithDraftStatus()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario Rossi", "Test description");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/generate?type=Proforma", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Status.Should().Be(DocumentStatus.Draft);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Complete_ShouldCompleteDocument_WhenValid()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Status.Should().Be(DocumentStatus.Completed);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Complete_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_Complete_ShouldReturnBadRequest_WhenDocumentIsNotDraft()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Send_ShouldSendDocument_WhenValid()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/send", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Status.Should().Be(DocumentStatus.Sent);

        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Send_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/send", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_Approve_ShouldApproveDocument_WhenValid()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        document.Send();
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/approve", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Status.Should().Be(DocumentStatus.Approved);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Approve_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/approve", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_Reject_ShouldRejectDocument_WhenValid()
    {
        // Given
        var document = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        document.Send();
        await TestDataSeeder.InsertDocumentAsync(_database, document);

        // When
        var response = await _client.PostAsync($"/documents/{document.Id}/reject", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.Status.Should().Be(DocumentStatus.Rejected);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Reject_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/reject", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DocumentsController_Link_ShouldLinkDocuments_WhenValid()
    {
        // Given
        var source = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        var target = new Domain.Models.Document(DocumentType.Proforma, "Luigi", "desc2");
        await TestDataSeeder.InsertDocumentAsync(_database, source);
        await TestDataSeeder.InsertDocumentAsync(_database, target);

        // When
        var response = await _client.PostAsync($"/documents/{source.Id}/link/{target.Id}", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DocumentResponse>();
        result.LinkedDocuments.Should().ContainSingle(id => id == target.Id);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Link_ShouldReturnNotFound_WhenSourceDocumentDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();
        var target = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        await TestDataSeeder.InsertDocumentAsync(_database, target);

        // When
        var response = await _client.PostAsync($"/documents/{nonExistingId}/link/{target.Id}", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }

    [Fact]
    public async Task DocumentsController_Link_ShouldReturnNotFound_WhenTargetDocumentDoesNotExist()
    {
        // Given
        var source = new Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        var nonExistingTargetId = ObjectId.GenerateNewId().ToString();
        await TestDataSeeder.InsertDocumentAsync(_database, source);

        // When
        var response = await _client.PostAsync($"/documents/{source.Id}/link/{nonExistingTargetId}", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await TestDataSeeder.ResetDocumentsAsync(_database);
    }
}