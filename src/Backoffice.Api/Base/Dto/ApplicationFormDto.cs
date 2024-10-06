using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed class ApplicationFormDto
{
    public ApplicationFormDto(ApplicationFormType name)
    {
        Name = name;
    }

    public ApplicationFormType Name { get; private set; }
    public bool Completed { get; private set; }

    public List<object> Items { get; private set; } = [];
    public Dictionary<string, object> Sections { get; private set; } = [];

    public void AddItem<T>(VerifiableSectionDto<T> item) where T : class
    {
        Items.Add(item);

        if (!Completed)
            Completed = true;
    }

    public void AddSection<T>(string name, VerifiableSectionDto<T> section) where T : class
    {
        Sections[name] = section;

        if (!Completed)
            Completed = true;
    }
}

public sealed class VerifiableSectionDto<T> where T : class
{
    public Guid? Id { get; init; }
    public bool Verified { get; set; }
    public T? Data { get; set; }

    public void Set(bool isVerified, T data)
    {
        Verified = isVerified;
        Data = data;
    }
}