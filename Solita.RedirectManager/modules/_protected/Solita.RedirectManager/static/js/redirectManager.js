(function() {
    'use strict';

    angular.module('EpiUrlRedirect', [
        'ngResource',
        'Solita.DojoWrapper'
    ]).controller('UrlRedirectController', function(SolDojo, $scope, $rootScope, $resource, $window, $log) {
        var UrlRedirectService; //constructed later, see $scope.init
        var urlRegExp = new RegExp('^((https?:\\/\\/)?([a-z\\d\\-\\.]*)?(\\/.*))$', 'i');
        var redirectUrlRegExp = new RegExp('^(https?:\\/\\/|\\/).*', 'i');

        $scope.selectedTab = 0;
        $scope.requestUrl = '';
        $scope.redirectUrl = '';
        $scope.redirectFilter = '';
        $scope.isWildcard = false;
        $scope.preservePath = false;
        $scope.redirectId = 0;

        $scope.selectTab = function(index) {
            $scope.selectedTab = index;
        };

        $scope.addRedirect = function(redirect, callback) {
            //if a redirect config object is provided (e.g. when reading a CSV file), ignore form inputs
            
            var requestUrl = redirect ? redirect.requestUrl : $scope.requestUrl;
            var redirectUrl = redirect ? redirect.redirectUrl : $scope.redirectUrl;
            var isWildcard = redirect ? redirect.isWildcard : $scope.isWildcard;
            var preservePath = redirect ? (redirect.isWildcard && redirect.preservePath) : ($scope.isWildcard && $scope.preservePath);

            if (!urlRegExp.test(requestUrl) || !redirectUrlRegExp.test(redirectUrl)) return;

            var match = requestUrl.match(urlRegExp);

            UrlRedirectService.findMapping({
                host: match[3] || null,
                path: match[4]
            }, function(response) {
                if (response.Id) {
                    if (angular.isFunction(callback)) {
                        return callback('redirectExists');
                    } else {
                        return $window.alert('Redirection from ' + requestUrl + ' already exists');
                    }
                }

                UrlRedirectService.create({
                    RequestPath: match[4],
                    RequestHost: match[3] || '',
                    RedirectUrl: redirectUrl,
                    IsWildcard: isWildcard,
                    PreservePath: preservePath
                }, function(response) {
                    //TODO: update if not sorted by latest created redirect
                    $scope.currentRedirects.splice(0, 0, parseRedirect(response));
                    if (angular.isFunction(callback)) callback();
                }, function(error) {
                    $log.error('UrlRedirectService::create()', error);
                    if (!angular.isFunction(callback)) {
                        $window.alert('Adding failed, please try again');
                    } else {
                        callback(true);
                    }
                });
            });
        };

        $scope.deleteRedirect = function(id, index) {
            if ($window.confirm('Delete redirection?')) {
                UrlRedirectService.delete({ id: id }, function() {
                    $scope.currentRedirects.splice(index, 1);
                }, function(error) {
                    $window.alert('Delete failed, please try again');
                    $log.error('UrlRedirectService::delete()', error);
                });
            }
        };

        $scope.$watchCollection('currentRedirects', function() {
            $scope.redecorate();
        });

        $scope.$watch('isWildcard', function() {
            if (!$scope.isWildcard) {
                $scope.preservePath = false;
            }
        });

        $scope.redecorate = function() {
            if ($scope.dojoLoaded && !$scope.decorating) {
                $scope.decorating = true;
                SolDojo.parse(function() {
                    $scope.decorating = false;
                });
            }
        };

        $scope.updateRedirect = function() {
            if ($scope.redirectPage) {
                $scope.redirectUrl = $scope.redirectPage.publicUrl;
            }
        };

        $scope.readCsv = function() {
            var parseRow = function(row) {
                //it is assumed that the columns are in the following order: requestUrl, redirectUrl, isWildcard, preservePath.
                //isWildcard and preservePath are optional
                if (row.length < 2 || row.length > 4) return null;

                return {
                    requestUrl: row[0],
                    redirectUrl: row[1],
                    isWildcard: row.length > 2 && !!row[2],
                    preservePath: row.length > 3 && (!!row[2] ? !!row[3] : false)
                };
            };

            if (!$('#csvInput')[0].files[0]) return;

            if (['application/vnd.ms-excel', 'text/plain', 'text/csv', 'text/tsv'].indexOf($('#csvInput')[0].files[0].type) === -1) {
                $window.alert('The filetype is unsupported');
                $('#csvInput')[0].value = null;
                return;
            }

            $scope.csvErrors = null;
            $scope.dojoLoaded = false;

            Papa.parse($('#csvInput')[0].files[0], {
                header: false,
                complete: function(results) {
                    processCsv(results.data.map(parseRow));
                    $scope.redecorate();
                }
            });
        };

        var processCsv = function(rows) {
            var csvErrors = [];

            (function processRow(index) {
                if (index === rows.length) {
                    if (csvErrors.length > 0) {
                        $scope.csvErrors = csvErrors;
                    }

                    $scope.dojoLoaded = true;
                    return;
                }

                if (!rows[index]) {
                    //empty row, ignore
                    return processRow(++index);
                }

                var redirect = rows[index];

                if (!urlRegExp.test(redirect.requestUrl)) {
                    csvErrors.push('Row ' + (index + 1) + ': invalid old URL');
                    return processRow(++index);
                } else if (!redirectUrlRegExp.test(redirect.redirectUrl)) {
                    csvErrors.push('Row ' + (index + 1) + ': invalid new URL');
                    return processRow(++index);
                }

                $scope.addRedirect(redirect, function(error) {
                    if (error) {
                        var errMsg = 'Row ' + (index + 1) + ': ';
                        
                        switch (error) {
                            case 'redirectExists': errMsg += 'redirection from ' + redirect.requestUrl + ' already exists'; break;
                            default: errMsg += 'error in saving (is the old URL already redirected?)';
                        }

                        csvErrors.push(errMsg);
                    }

                    return processRow(++index);
                });
            }(0));
        };

        $rootScope.$on('SolDojoInit', function() {
            $scope.dojoLoaded = true;
        })

        var initTabContents = function() {
            //initialize main tab
            UrlRedirectService.getAll(function(response) {
                $scope.currentRedirects = response.Rewritemappings.map(parseRedirect);
            }, function(error) {
                $log.error('UrlRedirectService::getAll()', error);
            });

            //TODO: add initialization for rest of the tabs
        };

        var parseRedirect = function(redirect) {
            try {
                var created = parseInt(redirect.CreatedAt.match(/-?\d+/)[0]);
                redirect.CreatedAt = created > 0 ? new Date(created) : null;
            } catch (e) {
                redirect.CreatedAt = null;
            }

            try {
                var lastUsed = parseInt(redirect.LastUsed.match(/-?\d+/)[0]);
                redirect.LastUsed = lastUsed > 0 ? new Date(lastUsed) : null;
            } catch (e) {
                redirect.LastUsed = null;
            }

            return redirect;
        };

        $scope.init = function(config) {
            //config passed via ng-init - WARNING: will overwrite any existing $scope properties
            Object.keys(config).forEach(function(key) {
                $scope[key] = config[key];
            });

            //now apiBaseUrl is known, construct service
            UrlRedirectService = $resource($scope.apiBaseUrl + '/:verb/:id', {}, {
                'getAll': { method: 'GET', params: { verb: 'GetAll' } },
                'get': { method: 'GET', params: { verb: 'Get' } },
                'create': { method: 'POST', params: { verb: 'Create' } },
                'edit': { method: 'PUT', params: { verb: 'Edit' } },
                'delete': { method: 'DELETE', params: { verb: 'Delete' } },
                'findMapping': { method: 'GET', params: { verb: 'FindMapping' } }
            });
            
            initTabContents();
        };
    });
}());