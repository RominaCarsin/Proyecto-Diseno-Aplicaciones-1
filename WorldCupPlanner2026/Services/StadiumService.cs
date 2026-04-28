using Domain;
using IRepository;
using IServices;

namespace Services;

public class StadiumService : IStadiumService
{
        private readonly IRepository<Stadium> _repository;
    
        public StadiumService(IRepository<Stadium> repository)
        {
            _repository = repository;
        }
    
       
        public void AddStadium(Stadium stadium)
        {
            bool nameExists = _repository.GetAll()
                .FirstOrDefault(s => s.Name.Equals(stadium.Name, StringComparison.OrdinalIgnoreCase)) != null;

            if (nameExists)
                throw new ArgumentException($"A stadium with the name '{stadium.Name}' already exists");

            _repository.Add(stadium);
        }

        public void DeleteStadium(string stadiumName)
        {
            stadiumName = stadiumName?.Trim() ?? string.Empty;
            var stadium = _repository.GetAll().FirstOrDefault(s => s.Name.Equals(stadiumName, StringComparison.OrdinalIgnoreCase));
    
            
            if (stadium == null)
                throw new ArgumentException($"A stadium with the name '{stadiumName}' does not exist");

            _repository.Delete(stadium);
        }
        
        public void UpdateStadium(string originalName, Stadium stadium)
        {
            originalName = originalName?.Trim() ?? string.Empty;
            var existingStadium = _repository.GetAll()
                .FirstOrDefault(s => s.Name.Equals(originalName, StringComparison.OrdinalIgnoreCase));
    
            if (existingStadium == null)
                throw new ArgumentException($"A stadium with the name '{originalName}' does not exist");
    
            bool nameExists = _repository.GetAll()
                .FirstOrDefault(s => s.Name.Equals(stadium.Name, StringComparison.OrdinalIgnoreCase)) != null;
    
            if (nameExists && !stadium.Name.Equals(originalName, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"A stadium with the name '{stadium.Name}' already exists");
    
            _repository.Delete(existingStadium);
            _repository.Add(stadium);
        }

        


    
        public IEnumerable<Stadium> GetStadiums()
        {
            return _repository.GetAll();
        }
}