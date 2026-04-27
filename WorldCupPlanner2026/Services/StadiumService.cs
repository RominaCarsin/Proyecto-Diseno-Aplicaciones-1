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
            _repository.Add(stadium);
        }
    
        public IEnumerable<Stadium> GetStadiums()
        {
            return _repository.GetAll();
        }
}