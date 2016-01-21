// Variables.
var app = angular.module("umbraco");

// Service to help with Formulate forms.
app.factory("formulateForms", function (formulateVars,
    formulateServer) {

    // Variables.
    var services = {
        formulateVars: formulateVars,
        formulateServer: formulateServer
    };

    // Return service.
    return {

        // Gets the form info for the form with the specified ID.
        getFormInfo: getGetFormInfo(services),

        // Saves the form on the server.
        persistForm: getPersistForm(services),

        // Deletes a form from the server.
        deleteForm: getDeleteForm(services)

    };

});

// Returns the function that gets information about a form.
function getGetFormInfo(services) {
    return function (id) {

        // Variables.
        var url = services.formulateVars.GetFormInfo;
        var params = {
            FormId: id
        };

        // Get form info from server.
        return services.formulateServer.get(url, params, function (data) {

            // Return form information.
            return {
                formId: data.FormId,
                alias: data.Alias,
                name: data.Name,
                path: data.Path,
                fields: data.Fields.map(function (field) {
                    return {
                        id: field.Id,
                        name: field.Name,
                        alias: field.Alias,
                        label: field.Label,
                        directive: field.Directive,
                        icon: field.Icon,
                        typeLabel: field.TypeLabel,
                        typeFullName: field.TypeFullName,
                        validations: field.Validations
                            .map(function(validation) {
                                return {
                                    id: validation.Id,
                                    name: validation.Name
                                };
                            }),
                        configuration: field.Configuration || {}
                    };
                })
            };

        });

    };
}

// Returns the function that persists a form on the server.
function getPersistForm(services) {
    return function (formData, isNew) {

        // Variables.
        var url = services.formulateVars.PersistForm;
        var data = {
            ParentId: formData.parentId,
            Alias: formData.alias,
            Name: formData.name,
            Fields: formData.fields.map(function(field) {
                var result = {
                    Name: field.name,
                    Alias: field.alias,
                    Label: field.label,
                    TypeFullName: field.typeFullName,
                    Validations: (field.validations || [])
                        .map(function(validation) {
                            return validation.id;
                        }),
                    Configuration: field.configuration
                };
                if (field.id) {
                    result.Id = field.id;
                }
                return result;
            })
        };
        if (!isNew) {
            data.FormId = formData.formId
        }

        // Send request to persist the form.
        return services.formulateServer.post(url, data, function (data) {

            // Return form ID.
            return {
                formId: data.FormId
            };

        });

    };
}

// Returns the function that deletes a form from the server.
function getDeleteForm(services) {
    return function(formId) {

        // Variables.
        var url = services.formulateVars.DeleteForm;
        var data = {
            FormId: formId
        };

        // Send request to delete the form.
        return services.formulateServer.post(url, data, function () {

            // Return empty data.
            return {};

        });

    };
}