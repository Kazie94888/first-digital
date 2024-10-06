using SmartCoinOS.Domain.Services.DocumentTypes;

namespace SmartCoinOS.Domain.Services;

public sealed class DocumentTypeService
{
    private static readonly Lazy<DocumentTypeService> _lazy = new(() => new DocumentTypeService());
    public static DocumentTypeService Instance => _lazy.Value;

    private IdentityProofDocumentType IdentityProof { get; } = new();
    private SourceOfWealthDocumentType SourceOfWealth { get; } = new();
    private BankDocumentType BankDocument { get; } = new();
    private EntityParticularDocumentType EntityParticular { get; } = new();
    private AddressProofDocumentType AddressProof { get; } = new();
    private DueDiligenceDocumentType DueDiligence { get; } = new();

    private DocumentTypeService()
    {
    }

    public Dictionary<string, IReadOnlyList<string>> GetAll()
    {
        return new Dictionary<string, IReadOnlyList<string>>
        {
            { IdentityProof.TypeName, IdentityProof.Items.Select(x => x.Name).ToList() },
            { SourceOfWealth.TypeName, SourceOfWealth.Items.Select(x => x.Name).ToList() },
            { BankDocument.TypeName, BankDocument.Items.Select(x => x.Name).ToList() },
            { EntityParticular.TypeName, EntityParticular.Items.Select(x => x.Name).ToList() },
            { AddressProof.TypeName, AddressProof.Items.Select(x => x.Name).ToList() },
            { DueDiligence.TypeName, DueDiligence.Items.Select(x => x.Name).ToList() },
        };
    }

    public bool IsValid(string documentType) => IsIdentityProof(documentType) || IsSourceOfWealth(documentType) ||
                                                IsBankDocument(documentType) || IsCompanyParticular(documentType) ||
                                                IsAddressProof(documentType) || IsDueDiligence(documentType);

    public bool IsIdentityProof(string docType) => IdentityProof.Found(docType);
    public bool IsSourceOfWealth(string docType) => SourceOfWealth.Found(docType);
    public bool IsBankDocument(string docType) => BankDocument.Found(docType);
    public bool IsCompanyParticular(string docType) => EntityParticular.Found(docType);
    public bool IsAddressProof(string docType) => AddressProof.Found(docType);
    public bool IsDueDiligence(string docType) => DueDiligence.Found(docType);
}