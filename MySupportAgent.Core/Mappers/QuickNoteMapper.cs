using Mapster;
using MySupportAgent.Core.Database.Entities;
using MySupportAgent.Core.Models;

namespace MySupportAgent.Core.Mappers;

public static class QuickNoteMapper
{
    private static readonly TypeAdapterConfig Config;

    static QuickNoteMapper()
    {
        Config = new TypeAdapterConfig();

        Config.NewConfig<QuickNoteEntity, QuickNoteItem>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content);

        Config.NewConfig<QuickNoteItem, QuickNoteEntity>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content);
    }

    public static QuickNoteItem ToModel(QuickNoteEntity entity)
    {
        return entity.Adapt<QuickNoteItem>(Config);
    }

    public static QuickNoteEntity ToEntity(QuickNoteItem model)
    {
        return model.Adapt<QuickNoteEntity>(Config);
    }
}
