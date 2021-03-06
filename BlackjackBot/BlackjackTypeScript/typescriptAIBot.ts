﻿/*
  TypeScript Blackjack AI Bot
  
  This file is a demonstration Blackjack AI bot.  It will connect to the server and 
  follow a basic Blackjack strategy.

  Edit this file with your AI bot logic
*/

declare var $: HubJQuery;

$.connection.blackjackHub.on("errorReceived", errorReceived);
$.connection.blackjackHub.on("gameStarted", gameStarted);
$.connection.blackjackHub.on("playerMove", playerMove);
$.connection.blackjackHub.on("gameCompleted", gameCompleted);

function errorReceived(errorMsg: string) {
    alert("error: " + errorMsg);
}

function gameStarted(gameState: GameState) {
    if (gameState.Me != null) {
        var amountToBet = gameState.Me.Balance / 10;
        $.connection.blackjackHub.server.placeBet(amountToBet);
        console.log("Bot: " + gameState.Me.Name + ", Called placeBet: " + amountToBet);
    }
}

function calculateMove(cardSum: number, canDoubleDown: boolean) {
    if (cardSum > 16) {
        return Move.Stand;
    }

    if ((cardSum == 10 || cardSum == 11) && canDoubleDown) {
        return Move.DoubleDown;
    }

    return Move.Hit;
}

function playerMove(gameState: GameState) {
    var move: Move;

    if (gameState.Me.Hand == null) {
        console.log(gameState.Me.Name + " has null hand");
    }

    var totals: number[] = HandUtils.GetSumOfHand(gameState.Me.Hand);
    if (HandUtils.ContainsCard(gameState.Me.Hand, FaceValue.Ace)) {
        if (totals[1] > 21) {
            move = calculateMove(totals[0], gameState.Me.CanDoubleDown);
        }
        else {
            move = calculateMove(totals[1], gameState.Me.CanDoubleDown);
        }
    }
    else {
        move = calculateMove(totals[0], gameState.Me.CanDoubleDown);
    }

    $.connection.blackjackHub.server.playerMoveAsync(move);
}

function gameCompleted(gameState: GameState) {
    console.log("balance: " + gameState.Me.Balance);
}

$.connection.hub.start().done(function () {
    //You can use the below line of a quick check of server connectivity
    //$.connection.blackjackHub.server.echo("TypeScriptEchoMessage"); // works!
    $.connection.blackjackHub.server.joinGameQueue("TypeScriptUser");
}).fail(function () {
    console.log('Could not Connect!');
});