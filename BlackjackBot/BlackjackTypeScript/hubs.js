/*!
 * ASP.NET SignalR JavaScript Library v2.0.2
 * http://signalr.net/
 *
 * Copyright Microsoft Open Technologies, Inc. All rights reserved.
 * Licensed under the Apache 2.0
 * https://github.com/SignalR/SignalR/blob/master/LICENSE.md
 *
 */

/// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
/// <reference path="jquery.signalR.js" />
(function ($, window, undefined) {
    /// <param name="$" type="jQuery" />
    "use strict";

    if (typeof ($.signalR) !== "function") {
        throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
    }

    var signalR = $.signalR;

    function makeProxyCallback(hub, callback) {
        return function () {
            // Call the client hub method
            callback.apply(hub, $.makeArray(arguments));
        };
    }

    function registerHubProxies(instance, shouldSubscribe) {
        var key, hub, memberKey, memberValue, subscriptionMethod;

        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!(hub.hubName)) {
                    // Not a client hub
                    continue;
                }

                if (shouldSubscribe) {
                    // We want to subscribe to the hub events
                    subscriptionMethod = hub.on;
                } else {
                    // We want to unsubscribe from the hub events
                    subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                    if (hub.client.hasOwnProperty(memberKey)) {
                        memberValue = hub.client[memberKey];

                        if (!$.isFunction(memberValue)) {
                            // Not a client hub function
                            continue;
                        }

                        subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                    }
                }
            }
        }
    }

    $.hubConnection.prototype.createHubProxies = function () {
        var proxies = {};
        this.starting(function () {
            // Register the hub proxies as subscribed
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, true);

            this._registerSubscribedHubs();
        }).disconnected(function () {
            // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, false);
        });

        proxies.blackjackHub = this.createHubProxy('blackjackHub');
        proxies.blackjackHub.client = {};
        proxies.blackjackHub.server = {
            clearLog: function () {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["ClearLog"], $.makeArray(arguments)));
            },

            echo: function (text) {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["Echo"], $.makeArray(arguments)));
            },

            getLog: function () {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["GetLog"], $.makeArray(arguments)));
            },

            joinGameQueue: function (name) {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["JoinGameQueue"], $.makeArray(arguments)));
            },

            placeBet: function (amount) {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["PlaceBet"], $.makeArray(arguments)));
            },

            playerMoveAsync: function (playerMove) {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["PlayerMoveAsync"], $.makeArray(arguments)));
            },

            startGameSeriesAsync: function () {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["StartGameSeriesAsync"], $.makeArray(arguments)));
            },

            startSoloGameSeriesAsync: function (name) {
                return proxies.blackjackHub.invoke.apply(proxies.blackjackHub, $.merge(["StartSoloGameSeriesAsync"], $.makeArray(arguments)));
            }
        };

        proxies.hubTester = this.createHubProxy('hubTester');
        proxies.hubTester.client = {};
        proxies.hubTester.server = {
            echo: function (text) {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["Echo"], $.makeArray(arguments)));
            },

            hello: function () {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["Hello"], $.makeArray(arguments)));
            },

            runThis: function () {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["RunThis"], $.makeArray(arguments)));
            },

            setTimer: function (value) {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["SetTimer"], $.makeArray(arguments)));
            },

            setupPlayerQueue: function () {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["SetupPlayerQueue"], $.makeArray(arguments)));
            },

            trySending: function (text) {
                return proxies.hubTester.invoke.apply(proxies.hubTester, $.merge(["TrySending"], $.makeArray(arguments)));
            }
        };

        return proxies;
    };

    signalR.hub = $.hubConnection("http://blackjackbotserver.azurewebsites.net/signalr", { useDefaultPath: false });
    $.extend(signalR, signalR.hub.createHubProxies());

}(window.jQuery, window));