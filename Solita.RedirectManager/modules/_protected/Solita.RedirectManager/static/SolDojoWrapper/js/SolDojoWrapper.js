/*
* To extend and improve this module you can find more information about this from these links:
*
* https://gist.github.com/mvirkkunen/10484486
*
* Content selector:
* http://world.episerver.com/forum/developer-forum/-EPiServer-75-CMS/Thread-Container/2015/6/epi-dojo-editor-widget-in-dashboard-gadget/
* Search from Google using: "epi-cms/widget/ContentSelector"
*
*/
(function() {

    'use strict';

    angular.module('Solita.DojoWrapper', [])
    /* Utilizing application should redefine the value below, otherwise template loading will fail */
    .value('SolDojoBasePath', '')
    .factory('SolDojo', function($rootScope, $timeout, $log) {
        var SolDojoservice = { registry: [] };
        var Initialized = false;

        /* SolDojoservice initialization callback */
        SolDojoservice.dojoInitialized = function() {
            Initialized = true;

            angular.forEach(SolDojoservice.registry, function(cb) {
                cb();
            });

            SolDojoservice.registry = [];
            $rootScope.$broadcast('SolDojoInit');
        };

        SolDojoservice.registerCallback = function(cb) {
            if (!angular.isFunction(cb)) {
                $log.warn('SolDojoservice::registerCallback, given callback function is not a function!', cb);
                return;
            }

            if (Initialized) {
                cb();
            }
            else {
                SolDojoservice.registry.push(cb);
            }
        };

        /* If new declarative widgets are added after initialization, run this method */
        SolDojoservice.parse = function(callback) {
            $timeout(function() {
                require(['dojo/parser'], function(parser) {
                    parser.parse().then(function() {
                        if (angular.isFunction(callback)) callback();
                    });
                });
            });
        };

        return SolDojoservice;
    })
    .directive('solDojoInitialization', function(SolDojo) {
        return {
            restrict: 'E',
            scope: {
                'initSettings': '@'
            },
            link: function(scope, element, attributes) {
                // At this point it's not safe to require() arbitrary things yet or everything will blow up spectacularly. The
                // "Bootstrapper" has to be run first, so only require that.
                require(['epi/shell/Bootstrapper'], function(Bootstrapper) {
                    var bs = new Bootstrapper(angular.fromJson(scope.initSettings));

                    // Loads the specified module ("CMS") and all the script bundles ClientResources that come with it. If this isn't done
                    // correctly all require() calls will load modules with separate requests which can reduce the amount of total code
                    // loaded but generates a *lot* of requests in the process
                    bs.initializeApplication(null, 'CMS').then(function() {
                        // It's now safe to require() anything including your own modules.
                        require([
                            'dojo/_base/connect',
                            'dojo/parser',
                            'epi-cms/ApplicationSettings'
                        ], function(
                            connect,
                            parser,
                            ApplicationSettings
                        ) {
                            // This sets the "current context" which is required by some controls such as the WYSIWYG.
                            // It's used to show the current page media list as well as the "Current page" button in page selectors. This
                            // just sets it to the root page so everything doesn't break.
                            connect.publish('/epi/shell/context/updateRequest', [{ uri: 'epi.cms.contentdata:///' + ApplicationSettings.rootPage }]);
                            // All done! Everything should be set up now. Run your own code here.

                            // Should probably run this at some point as it's not done automatically - this initializes all the declarative
                            // widgets (elements with data-dojo-type). Use .then() if you want to run code after this to ensure everything has
                            // finished executing.
                            parser.parse().then(SolDojo.dojoInitialized);
                        });
                    });
                });
            }
        }
    })
    .directive('solDojoContent', function(SolDojo, SolDojoBasePath, $log) {
        return {
            restrict: 'E',
            templateUrl: function(element, attributes) {
                if (!/^(folder|page)$/.test(attributes.contentType)) {
                    $log.error('solDojoContent, given content type is not supported!', attributes.contentType);
                    attributes.contentType = 'folder';
                }

                return SolDojoBasePath + '/templates/sol-dojo-content-' + attributes.contentType + '.html'
            },
            require: 'ngModel',
            scope: {
                contentType: '@',
                allowedRoots: '@',
                allowedTypes: '@'
            },
            link: function(scope, element, attributes, ngModelCtrl) {
                SolDojo.registerCallback(function() {
                    require([
                        'dojo/_base/connect',
                        'dojo/query',
                        'dojo/when',
                        'dijit/registry',
                        'epi-cms/core/PermanentLinkHelper'
                    ], function(
                        connect,
                        query,
                        when,
                        registry,
                        PermanentLinkHelper
                    ) {
                        connect.connect(registry.byId('contentSelector-' + scope.$id), 'onChange', function(link) {
                            if (link === null) {
                                scope.$apply(function() {
                                    ngModelCtrl.$setViewValue(null);
                                });
                                return;
                            }

                            when(PermanentLinkHelper.getContent(link), function(content) {
                                scope.$apply(function() {
                                    ngModelCtrl.$setViewValue(content);
                                });
                            });
                        });
                    });
                });
            }
        }
    })
    .directive('solDojoButton', function(SolDojoBasePath) {
        return {
            restrict: 'E',
            templateUrl: SolDojoBasePath + '/templates/sol-dojo-button.html',
            scope: {
                iconClass: '@'
            },
            transclude: true,
            link: function(scope, element, attributes, ngModelCtrl) {
            }
        }
    })
    .directive('solDojoTextbox', function(SolDojo, SolDojoBasePath) {
        return {
            restrict: 'E',
            templateUrl: SolDojoBasePath + '/templates/sol-dojo-textbox.html',
            require: 'ngModel',
            scope: {
                properties: '@',
                //validationExp should be a string, in the same format as supported by new RegExp()
                validationExp: '@',
                validationMsg: '@'
            },
            link: function(scope, element, attributes, ngModelCtrl) {
                if (scope.validationExp) {
                    scope.properties = 'regExp:\'' + scope.validationExp + '\', invalidMessage:\'' + scope.validationMsg + '\'' +
                        (scope.properties ? ', ' + scope.properties : '');
                }

                SolDojo.registerCallback(function() {
                    require([
                        'dojo/_base/connect',
                        'dijit/registry'
                    ], function(
                        connect,
                        registry
                    ) {
                        connect.connect(registry.byId('textbox-' + scope.$id), 'onKeyUp', function(event) {
                            var textbox = this;

                            scope.$apply(function() {
                                ngModelCtrl.$setViewValue(textbox.getValue());
                            });
                        });

                        /* reflect changes in model to textbox */
                        ngModelCtrl.$render = function() {
                            registry.byId('textbox-' + scope.$id).setValue(ngModelCtrl.$modelValue);
                        }

                        /* if readonly attribute evaluates to true, set textbox readonly */
                        /* note that the attribute is passed as a string, thus any non-empty string will evaluate as true */
                        if (attributes.readonly) {
                            registry.byId('textbox-' + scope.$id).setDisabled(true);
                        }

                        attributes.$observe('readonly', function(value) {
                            registry.byId('textbox-' + scope.$id).setDisabled(!!value);
                        });
                    });
                });
            }
        }
    })
    .directive('solDojoCheckbox', function(SolDojo, SolDojoBasePath) {
        return {
            restrict: 'E',
            templateUrl: SolDojoBasePath + '/templates/sol-dojo-checkbox.html',
            require: 'ngModel',
            scope: {},
            transclude: true,
            link: function(scope, element, attributes, ngModelCtrl) {
                SolDojo.registerCallback(function() {
                    require([
                        'dojo/_base/connect',
                        'dijit/registry'
                    ], function(
                        connect,
                        registry
                    ) {
                        connect.connect(registry.byId('checkbox-' + scope.$id), 'onChange', function() {
                            var checkbox = this;

                            scope.$apply(function() {
                                ngModelCtrl.$setViewValue(checkbox.checked);
                            });
                        });

                        /* reflect changes in model to checkbox */
                        ngModelCtrl.$render = function() {
                            registry.byId('checkbox-' + scope.$id).checked = ngModelCtrl.$modelValue;
                        }

                        /* if readonly attribute evaluates to true, set checkbox readonly */
                        /* note that the attribute is passed as a string, thus any non-empty string will evaluate as true */
                        if (attributes.readonly) {
                            registry.byId('checkbox-' + scope.$id).setDisabled(true);
                        }

                        attributes.$observe('readonly', function(value) {
                            registry.byId('checkbox-' + scope.$id).setDisabled(!!value);
                        });
                    });
                });
            }
        }
    });
}());