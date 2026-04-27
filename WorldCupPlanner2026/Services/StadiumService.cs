using Domain;
using IRepository;

namespace Services;

public class StadiumService
{
        private readonly IRepository<Stadium> _repository;
    
        public StadiumService(IRepository<Stadium> repository)
        {
            _repository = repository;
        }
    
       
        public void AddStadium(Stadium stadium)
        {
            bool nameExists = _repository.GetAll()
                .FirstOrDefault(s => s.Name == stadium.Name) != null;

            if (nameExists)
                throw new ArgumentException($"A stadium with the name '{stadium.Name}' already exists");

            _repository.Add(stadium);
        }

        public void DeleteStadium(string stadiumName)
        {
            var stadium = _repository.GetAll().FirstOrDefault(s => s.Name == stadiumName);
    
            
            if (stadium == null)
                throw new ArgumentException($"A stadium with the name '{stadiumName}' does not exist");

            _repository.Delete(stadium);
        }
        
        public void UpdateStadium(string originalName, Stadium stadium)
        {
            var existingStadium = _repository.GetAll()
                .FirstOrDefault(s => s.Name == originalName);
            
            if (existingStadium == null)
                throw new ArgumentException($"A stadium with the name '{originalName}' does not exist");
            
            _repository.Delete(existingStadium);
            _repository.Add(stadium);
        }
        


    
        public IEnumerable<Stadium> GetStadiums()
        {
            return _repository.GetAll();
        }
}