namespace ApiGeneratorApi.Models
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
            new BusinessLogicGenerator(_endpointSpec, modelType).Generate();
            new DataAccessGenerator(_endpointSpec, modelType).Generate();
            new TestGenerator(_endpointSpec, modelType).Generate();
        }
    }
}