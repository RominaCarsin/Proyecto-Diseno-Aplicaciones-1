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
    
            if (stadium != null)
                _repository.Delete(stadium);
        }


    
        public IEnumerable<Stadium> GetStadiums()
        {
            return _repository.GetAll();
        }
}