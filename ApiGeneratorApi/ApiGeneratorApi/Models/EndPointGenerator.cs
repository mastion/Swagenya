using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace ApiGeneratorApi.Models
{
    public class EndPointGenerator
    {
        private EndpointSpec _endpointSpec;

        public EndPointGenerator(EndpointSpec endpointSpec)
        {
            _endpointSpec = endpointSpec;
        }
         
        public void Generate()
        {
            var modelGenerator = new ModelGenerator(_endpointSpec);
            modelGenerator.Generate();
            var modelType = modelGenerator.GetType();
            new AngularGenerator(_endpointSpec, modelType).Generate();
            new WebApiGenerator(_endpointSpec, modelType).Generate();
            new BusinessLogicGenerator(_endpointSpec, modelType).Generate();
            new DataAccessGenerator(_endpointSpec, modelType).Generate();
            new TestGenerator(_endpointSpec, modelType).Generate();
        }
    }
}