/// <reference path="../_references.js" />
my.ApiClient = function (config) {
    var authToken = config.authToken,
        baseUri = config.baseUri,
        configureRequest = function (xhr) {
            xhr.setRequestHeader("Authorization", "Session " + authToken);
        },
        errorHandler = function (xhr, status, error) {
            if (xhr.status === 401 && config.onAuthFail) {
                config.onAuthFail(xhr, status, error);
            }
        };

    // TODO invoke error handler when preflight request fails

    this.createUri = function (path) {
        return baseUri + "/" + path;
    };

    this.get = function (path, query) {
        return $.ajax({
            url: this.createUri(path),
            type: "GET",
            beforeSend: configureRequest
        });
    };

    this.post = function (path, data) {
        return $.ajax({
            url: this.createUri(path),
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: configureRequest
        });
    };

    this.put = function (path, data) {
        return $.ajax({
            url: this.createUri(path),
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: configureRequest
        });
    };

    this.delete = function (path) {
        return $.ajax({
            url: this.createUri(path),
            type: "DELETE",
            dataType: "json",
            beforeSend: configureRequest
        });
    }
};
