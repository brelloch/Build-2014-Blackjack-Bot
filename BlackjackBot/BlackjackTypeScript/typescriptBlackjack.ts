/*********************************
 TypeScript Blackjack Support File
**********************************/

/*
  This file handles the basic support for Blackjack and for connecting to the Blackjack
  server.

  You should not need to edit this file.  Instead, edit the typescriptAIBot.ts file to include
  your AI bot logic.
*/

declare enum EndResult {
    DealerBlackJack = 0,
    PlayerBlackJack = 1,
    PlayerBust = 2,
    DealerBust = 3,
    Push = 4,
    PlayerWin = 5,
    DealerWin = 6,
    PlayerMoveTimeout = 7
}

declare enum Move {
    Hit = 0,
    Stand = 1,
    DoubleDown = 2,
    PayToSeeDealersCard = 3
}

declare enum Suit { Diamonds = 0, Spades = 1, Clubs = 2, Hearts = 3 }

declare enum FaceValue {
    Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
    Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14
}

interface Card {
    Suit: Suit;
    FaceVal: FaceValue;

    new (Suit: Suit, FaceValue: FaceValue);
}

interface Hand {
    NumCards: number;
    Cards: Array<Card>;
}

module HandUtils {
    export function ContainsCard(hand: Hand, item: FaceValue): boolean {
        for (var i in hand.Cards) {
            var c = hand.Cards[i];
            if (c.FaceVal == item) {
                return true;
            }
        }
        return false;
    }
    export function GetBestHand(hand: Hand): number {
        var handValues = GetSumOfHand(hand);

        if (handValues[1] <= 21) {
            //send back ace high value that isn't busted
            return handValues[1];
        }
        else {
            //send back ace low value (may be same as above or a bust too)
            return handValues[0];
        }
    }

    export function GetSumOfHand(hand: Hand): number[] {
        var returnValue = [0, 0];
        var highVal = 0;
        var lowVal = 0;
        var numAces = 0;

        for (var i in hand.Cards) {
            var c = hand.Cards[i];
            if (c.FaceVal == FaceValue.Jack || c.FaceVal == FaceValue.Queen || c.FaceVal == FaceValue.King) {
                lowVal += 10;
            }
            else if (c.FaceVal == FaceValue.Ace) {
                numAces++;
                lowVal += 1;
            }
            else {
                lowVal += c.FaceVal;
            }
        }

        if (numAces > 0) {
            highVal = lowVal + 10;
        }
        else {
            highVal = lowVal;
        }

        returnValue[0] = lowVal;
        returnValue[1] = highVal;

        return returnValue;
    }
    export function HasBlackJack(hand: Hand): boolean {
        if (hand.Cards.length == 2 && GetBestHand(hand) == 21) {
            return true;
        }
        return false;
    }
}

interface PlayerState {
    TimeQueued: any;
    PlayerId: string;
    GameSeriesId: string;
    CanDoubleDown: boolean;
    PaidToSeeDealersCard: boolean;
    TurnCompleted: boolean;
    Email: string;
    Name: string;
    Bet: number;
    MoveTimeout: boolean;
    Balance: number;
    Result: EndResult;
    Hand: Hand;
    Key: string;
}

interface GameState {
    GameSeriesId: string;
    GameNumber: number;
    Me: PlayerState;
    AllPlayers: Array<PlayerState>;
}

interface HubServer {
    clearLog();
    echo(msg: string);
    getLog();
    joinGameQueue(userName: string);
    placeBet(amt: number);
    playerMoveAsync(playerMove);
    startGameSeriesAsync();
    startSoloGameSeriesAsync(userName: string);
}

interface HubClient {
    gameStarted(gameState: GameState);
    playerMove(gameState: GameState);
    gameCompleted(gameState: GameState);
    deckShuffled();
    informationReceived(msg: string);
    errorReceived(errorMsg: string);
}

interface HubProxy {
    server: HubServer;
    client: HubClient;
    on(msg: string, func: Function);
}

interface Hub {
    start();
    id: any;
}

interface HubConnection {
    blackjackHub: HubProxy;
    hub: Hub;
}

interface HubJQuery {
    connection: HubConnection;
}
 