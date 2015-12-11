namespace ApiGeneratorApi.Models
{
    public class TestGenerator
    {
        private EndpointSpec _endPointSpec;

        public TestGenerator(EndpointSpec endpointSpec, string modelType)
        {
            _endPointSpec = endpointSpec;
        }

        public void Generate()
        {
            
        }
    }
}