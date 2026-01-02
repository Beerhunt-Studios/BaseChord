using BaseChord.Domain;

namespace BaseChord.IntegrationTests.Seeder;

public interface ISeeder
{
    List<IEntity> Seed();
}