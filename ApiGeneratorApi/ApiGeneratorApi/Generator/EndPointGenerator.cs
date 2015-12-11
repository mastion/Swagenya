using ApiGeneratorApi.Models;

namespace ApiGeneratorApi.Generator
{
    public class EndPointGenerator
    {
        private readonly EndpointSpec _endpointSpec;

        public EndPointGenerator(EndpointSpec endpointSpec)
        {
            _endpointSpec = endpointSpec;
        }

        public void Generate()
        {
            var modelGenerator = new ModelGenerator(_endpointSpec);
            modelGenerator.Generate();
            string modelType = modelGenerator.GetType();
            new ActionGenerator(_endpointSpec, modelType).Generate();
            new DataAccessGenerator(_endpointSpec, modelType).Generate();
            new StoredProcGenerator(_endpointSpec, modelType).Generate();
            new TestGenerator(_endpointSpec, modelType).Generate();
        }
    }
}