using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApiGeneratorApi.Models;
using ApiGeneratorApi.Util;

namespace ApiGeneratorApi.Generator
{
    public class AngularGenerator
    {
        private readonly IEnumerable<EndpointSpec> _endpointSpecifications;
        private readonly IFileWriter _fileWriter;
        private IEnumerable<ResourceSpec> _resources;
        private string _outputDirectory;

        public AngularGenerator(IEnumerable<ResourceSpec> resources)
        {
            _fileWriter = new FileWriter();
            _resources = resources;
            _outputDirectory = String.Format(@"{0}", new FolderWriter().GetFolderName("js"));
        }

        public void Generate()
        {
            foreach (var resourceSpec in _resources)
            {
                GenerateAngularApp(resourceSpec); //GOOD
                GenerateAngularService(resourceSpec); //GOOD except we need to figure out the URL from URI
                GenerateAngularController(resourceSpec); //GOOD
                GenerateAngularModel(resourceSpec);  
            }
        }

        private void GenerateAngularModel(ResourceSpec resourceSpec)
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("function {0}(data){{", resourceSpec.ResourceName));
            builder.AppendLine(String.Format("  if (!data){{")); //SOOO JANKY
            var defaultRequestFields = resourceSpec.GetModel;
            if (defaultRequestFields != null)
            {
                //Definitely janky here
                foreach (var property in defaultRequestFields)
                {
                    builder.AppendLine(String.Format("      this.{0} = null;", property.Name));
                }
                builder.AppendLine(String.Format("  }}"));
                foreach (var property in defaultRequestFields)
                {
                    builder.AppendLine(String.Format("  this.{0} = data.{0} || null;", property.Name));
                }
            }
            builder.AppendLine(String.Format("}}"));

            builder.AppendLine(String.Format("function {0}s(data){{\n" +
                                             "  var {0}s = [];\n" +
                                             "  if (data){{\n" +
                                             "      {0}s = data.map(function(single{0}){{\n" +
                                             "          return new {0}(single{0});\n" +
                                             "      }});\n" +
                                             "  }}\n" +
                                             "  return {0}s;\n" +
                                             "}}", resourceSpec.ResourceName));

            builder.AppendLine(String.Format("angular.module('{0}').value('{0}', {0}).value('{0}s', {0}s);", resourceSpec.ResourceName));

            _fileWriter.WriteFile(String.Format(@"{0}\{1}.js", _outputDirectory, resourceSpec.ResourceName), builder.ToString());
        }

        private void GenerateAngularController(ResourceSpec resourceSpec)
        {
            var builder = new StringBuilder();

            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Controller($scope, {0}Service) {{", resourceSpec.ResourceName));

            foreach (var endpointSpec in _endpointSpecifications)
            {
                if (String.Compare(endpointSpec.HttpVerb, "POST", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  $scope.add{0} = function() {{", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      {0}Service", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("          .add{0}($scope.{0}ToAdd)", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("          .then(function (response) {{"));
                    builder.AppendLine(String.Format("              $scope.current{0}s.push(new {0}(response.data);",
                        resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("              $scope.{0}ToAdd = {{}}", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("          }}, function (error) {{"));
                    builder.AppendLine(String.Format("              $scope.ErrorMessage = 'Error adding the new {0}';",
                        resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("          }});"));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  $scope.get{0}s = function() {{\n" +
                                                     "      $scope.current{0}s = [];\n" +
                                                     "      {0}Service.get{0}s()\n" +
                                                     "      then(function(response){{\n" +
                                                     "          $scope.current{0}s = new {0}s(response.data);\n" +
                                                     "      }}, function (error){{\n" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;\n" +
                                                     "      }});\n" +
                                                     "  }};", resourceSpec.ResourceName));

                }
                if (String.Compare(endpointSpec.HttpVerb, "PUT", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  $scope.update{0} = function({0}updated, index) {{\n" +
                                                     "      {0}Service.update{0}({0}updated)\n" +
                                                     "      .then(function(response){{\n" +
                                                     "          $scope.current{0}s[index] = {0}updated;\n" +
                                                     "      }}, function(error){{\n" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;\n" +
                                                     "      }});\n" +
                                                     "  }};", resourceSpec.ResourceName));
                }
                if (String.Compare(endpointSpec.HttpVerb, "DELETE", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  $scope.delete{0} = function(index){{\n" +
                                                     "      {0}Service.delete{0}(current{0}s[index])\n" +
                                                     "      .then(function(response){{\n" +
                                                     "          $scope.current{0}s.splice(index, 1);\n" +
                                                     "      }}, function(error){{\n" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;\n" +
                                                     "      }});\n" +
                                                     "  }};", resourceSpec.ResourceName));
                }
            }


            builder.AppendLine(String.Format("}};"));
            builder.AppendLine(String.Format("angular.module('{0}').controller('{0}Controller', ['$scope', '{0}Service', {0}Controller]);", resourceSpec.ResourceName));



            _fileWriter.WriteFile(String.Format(@"{0}\{1}Controller.js", _outputDirectory, resourceSpec.ResourceName), builder.ToString());
        }

        private void GenerateAngularService(ResourceSpec resourceSpec)
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Service($http) {{", resourceSpec.ResourceName));

            foreach (var endpointSpec in resourceSpec.Endpoints)
            {
                if (String.Compare(endpointSpec.HttpVerb, "POST", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.add{0} = function ({0}) {{", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: '{0}',", endpointSpec.HttpVerb));
                    builder.AppendLine(String.Format("          data: {0}", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.get{0}s = function () {{", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      return $http.get('{0}');", endpointSpec.Uri));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "PUT", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.update{0} = function({0}) {{", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: 'PUT',"));
                    builder.AppendLine(String.Format("          data: {0}", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }}"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "DELETE", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.delete{0} = function({0}) {{", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: 'DELETE',"));
                    builder.AppendLine(String.Format("          params: {0}", resourceSpec.ResourceName));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }}"));
                }
            }


            builder.AppendLine(String.Format("}}"));
            builder.AppendLine(String.Format("angular.module('{0}').service('{0}Service', ['$http', {0}Service]);",
                resourceSpec.ResourceName));



            _fileWriter.WriteFile(String.Format(@"{0}\{1}Service.js", _outputDirectory, resourceSpec.ResourceName), builder.ToString());
        }

        private void GenerateAngularApp(ResourceSpec resourceSpec)
        {
            var sb = new StringBuilder();
            sb.AppendLine("'use strict'");
            sb.AppendLine(String.Format("angular.module('{0}', []);", resourceSpec.ResourceName));
            _fileWriter.WriteFile(String.Format(@"{0}\{1}App.js", _outputDirectory, resourceSpec.ResourceName), sb.ToString());
        }
    }
}