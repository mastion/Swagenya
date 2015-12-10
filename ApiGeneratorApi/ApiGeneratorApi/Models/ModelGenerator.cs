namespace ApiGeneratorApi.Models
{
    public class ModelGenerator
    {
        private EndpointSpec _apiSpecification;
        public ModelGenerator(EndpointSpec apiSpecification)
        {
            _apiSpecification = apiSpecification;
        }

        public string GetType()
        {
            return _apiSpecification.Uri;
        }

        public void Generate()
        {
            
        }
    }
}