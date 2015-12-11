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
            GenerateAngularApp();
            GenerateAngularService();
            GenerateAngularController();
            GenerateAngularModel();

        }

        private void GenerateAngularModel()
        {
            var builder = new StringBuilder();
            builder.AppendLine(String.Format("function {0}(data){{", _modelType));
            builder.AppendLine(String.Format("  if (!data){{"));
            var defaultRequestFields = _endpointSpecifications.FirstOrDefault(p => p.HttpVerb == "GET") ??
                                       _endpointSpecifications.FirstOrDefault(p => p.HttpVerb == "POST");
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

            builder.AppendLine(String.Format("function {0}s(data){{" +
                                             "  var {0}s = [];" +
                                             "  if (data){{" +
                                             "      {0}s = data.map(function(single{0}){{" +
                                             "          return new {0}(single{0});" +
                                             "      }});" +
                                             "  }}" +
                                             "  return {0}s;" +
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
                if (endpointSpec.HttpVerb == "POST")
                {
                    builder.AppendLine(String.Format("  $scope.add{0} = function() {{", _modelType));
                    builder.AppendLine(String.Format("      {0}Service", _modelType));
                    builder.AppendLine(String.Format("          .add{0}($scope.{0}ToAdd)", _modelType));
                    builder.AppendLine(String.Format("          .then(function (response) {{"));
                    builder.AppendLine(String.Format("              $scope.active{0}s.push(new {0}(response.data);",
                        _modelType));
                    builder.AppendLine(String.Format("              $scope.{0}ToAdd = {{}}", _modelType));
                    builder.AppendLine(String.Format("          }}, function (error) {{"));
                    builder.AppendLine(String.Format("              $scope.ErrorMessage = 'Error adding the new {0}';",
                        _modelType));
                    builder.AppendLine(String.Format("          }});"));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (endpointSpec.HttpVerb == "GET")
                {
                    builder.AppendLine(String.Format("  $scope.get{0}s = function() {{" +
                                                     "      $scope.current{0}s = [];" +
                                                     "      {0}Service.get{0}s()" +
                                                     "      then(function(response){{" +
                                                     "          $scope.current{0}s = new {0}s(response.data);" +
                                                     "      }}, function (error){{" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;" +
                                                     "      }});" +
                                                     "  }};", _modelType));

                }
                if (endpointSpec.HttpVerb == "PUT")
                {
                    builder.AppendLine(String.Format("  $scope.update{0} = function({0}updated, index) {{" +
                                                     "      {0}Service.update{0}({0}updated)" +
                                                     "      .then(function(response){{" +
                                                     "          $scope.current{0}s[index] = {0}updated;" +
                                                     "      }}, function(error){{" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;" +
                                                     "      }});" +
                                                     "  }};", _modelType));
                }
                if (endpointSpec.HttpVerb == "DELETE")
                {
                    builder.AppendLine(String.Format("  $scope.delete{0} = function(index){{" +
                                                     "      {0}Service.delete{0}(current{0}s[index])" +
                                                     "      .then(function(response){{" +
                                                     "          $scope.current{0}s.splice(index, 1);" +
                                                     "      }}, function(error){{" +
                                                     "          $scope.ErrorMessage = error.data.ExceptionMessage;" +
                                                     "      }});" +
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
                if (endpointSpec.HttpVerb == "POST")
                {
                    builder.AppendLine(String.Format("  this.add{0} = function ({0}) {{", _modelType));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: '{0}',", endpointSpec.HttpVerb));
                    builder.AppendLine(String.Format("          data: '{0}'", _modelType));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (endpointSpec.HttpVerb == "GET")
                {
                    builder.AppendLine(String.Format("  this.get{0}s = function () {{", _modelType));
                    builder.AppendLine(String.Format("      return $http.get('{0}');", endpointSpec.Uri));
                    builder.AppendLine(String.Format("  }};"));
                }
                if (endpointSpec.HttpVerb == "PUT")
                {
                    builder.AppendLine(String.Format("  this.update{0} = function({0}) {{", _modelType));
                    builder.AppendLine(String.Format("      return $http({{"));
                    builder.AppendLine(String.Format("          url: '{0}',", endpointSpec.Uri));
                    builder.AppendLine(String.Format("          method: 'PUT',"));
                    builder.AppendLine(String.Format("          data: {0}", _modelType));
                    builder.AppendLine(String.Format("      }});"));
                    builder.AppendLine(String.Format("  }}"));
                }
                if (endpointSpec.HttpVerb == "DELETE")
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

        public void Generate2()
        {
            throw new NotImplementedException();
        }
    }
}