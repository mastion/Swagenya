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
        private readonly string _outputDirectory;
        private readonly string _modelType;
        private readonly IEnumerable<EndpointSpec> _endpointSpecifications;
        private readonly IFileWriter _fileWriter;

        public AngularGenerator(IEnumerable<EndpointSpec> endpoints)
        {
            _fileWriter = new FileWriter();
            _endpointSpecifications = endpoints;
            _modelType = new ModelGenerator(endpoints.FirstOrDefault()).GetType();
            _outputDirectory = String.Format(@"{0}", new FolderWriter().GetFolderName("js"));
        }

        public void Generate()
        {
            GenerateAngularApp(); //GOOD
            GenerateAngularService(); //GOOD except we need to figure out the URL from URI
            GenerateAngularController(); //GOOD
            GenerateAngularModel();

        }

        private void GenerateAngularModel()
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("function {0}(data){{", _modelType));
            builder.AppendLine(String.Format("  if (!data){{")); //SOOO JANKY
            var defaultRequestFields = _endpointSpecifications.FirstOrDefault(p => String.Compare(p.HttpVerb, "POST", StringComparison.OrdinalIgnoreCase) == 0) ??
                                       _endpointSpecifications.FirstOrDefault(p => String.Compare(p.HttpVerb, "GET", StringComparison.OrdinalIgnoreCase) == 0);
            if (defaultRequestFields != null)
            {
                //Definitely janky here
                foreach (var property in defaultRequestFields.Request)
                {
                    builder.AppendLine(String.Format("      this.{0} = null;", property.Name));
                }
                builder.AppendLine(String.Format("  }}"));
                foreach (var property in defaultRequestFields.Request)
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
                                             "}}", _modelType));

            builder.AppendLine(String.Format("angular.module('{0}').value('{0}', {0}).value('{0}s', {0}s);", _modelType));

            _fileWriter.WriteFile(String.Format(@"{0}\{1}.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularController()
        {
            var builder = new StringBuilder();

            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Controller($scope, {0}Service) {{", _modelType));

            foreach (var endpointSpec in _endpointSpecifications)
            {
                if (String.Compare(endpointSpec.HttpVerb, "POST", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  $scope.add{0} = function() {{", _modelType));
                    builder.AppendLine(String.Format("      {0}Service", _modelType));
                    builder.AppendLine(String.Format("          .add{0}($scope.{0}ToAdd)", _modelType));
                    builder.AppendLine(String.Format("          .then(function (response) {{"));
                    builder.AppendLine(String.Format("              $scope.current{0}s.push(new {0}(response.data);",
                        _modelType));
                    builder.AppendLine(String.Format("              $scope.{0}ToAdd = {{}}", _modelType));
                    builder.AppendLine(String.Format("          }}, function (error) {{"));
                    builder.AppendLine(String.Format("              $scope.ErrorMessage = 'Error adding the new {0}';",
                        _modelType));
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
                                                     "  }};", _modelType));

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
                                                     "  }};", _modelType));
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
                                                     "  }};", _modelType));
                }
            }


            builder.AppendLine(String.Format("}};"));
            builder.AppendLine(String.Format("angular.module('{0}').controller('{0}Controller', ['$scope', '{0}Service', {0}Controller]);", _modelType));



            _fileWriter.WriteFile(String.Format(@"{0}\{1}Controller.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularService()
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("'use strict'"));
            builder.AppendLine(String.Format("function {0}Service($http) {{", _modelType));

            foreach (var endpointSpec in _endpointSpecifications)
            {
                if (String.Compare(endpointSpec.HttpVerb, "POST", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.add{0} = function ({0}) {{", _modelType));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: '{0}',", endpointSpec.HttpVerb));
                    builder.AppendLine(String.Format("          data: {0}", _modelType));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.get{0}s = function () {{", _modelType));
                    builder.AppendLine(String.Format("      return $http.get('{0}');", endpointSpec.Uri));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "PUT", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.update{0} = function({0}) {{", _modelType));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: 'PUT',"));
                    builder.AppendLine(String.Format("          data: {0}", _modelType));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }}"));
                }
                if (String.Compare(endpointSpec.HttpVerb, "DELETE", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    builder.AppendLine(String.Format("  this.delete{0} = function({0}) {{", _modelType));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: 'DELETE',"));
                    builder.AppendLine(String.Format("          params: {0}", _modelType));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }}"));
                }
            }


            builder.AppendLine(String.Format("}}"));
            builder.AppendLine(String.Format("angular.module('{0}').service('{0}Service', ['$http', {0}Service]);",
                _modelType));



            _fileWriter.WriteFile(String.Format(@"{0}\{1}Service.js", _outputDirectory, _modelType), builder.ToString());
        }

        private void GenerateAngularApp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("'use strict'");
            sb.AppendLine(String.Format("angular.module('{0}', []);", _modelType));
            _fileWriter.WriteFile(String.Format(@"{0}\{1}App.js", _outputDirectory, _modelType), sb.ToString());
        }
    }
}