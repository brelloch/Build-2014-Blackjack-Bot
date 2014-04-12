/*********************************
TypeScript Blackjack Support File
**********************************/

var HandUtils;
(function (HandUtils) {
    function ContainsCard(hand, item) {
        for (var i in hand.Cards) {
            var c = hand.Cards[i];
            if (c.FaceVal == item) {
                return true;
            }
        }
        return false;
    }
    HandUtils.ContainsCard = ContainsCard;
    function GetBestHand(hand) {
        var handValues = GetSumOfHand(hand);

        if (handValues[1] <= 21) {
            //send back ace high value that isn't busted
            return handValues[1];
        } else {
            //send back ace low value (may be same as above or a bust too)
            return handValues[0];
        }
    }
    HandUtils.GetBestHand = GetBestHand;

    function GetSumOfHand(hand) {
        var returnValue = [0, 0];
        var highVal = 0;
        var lowVal = 0;
        var numAces = 0;

        for (var i in hand.Cards) {
            var c = hand.Cards[i];
            if (c.FaceVal == 11 /* Jack */ || c.FaceVal == 12 /* Queen */ || c.FaceVal == 13 /* King */) {
                lowVal += 10;
            } else if (c.FaceVal == 14 /* Ace */) {
                numAces++;
                lowVal += 1;
            } else {
                lowVal += c.FaceVal;
            }
        }

        if (numAces > 0) {
            highVal = lowVal + 10;
        } else {
            highVal = lowVal;
        }

        returnValue[0] = lowVal;
        returnValue[1] = highVal;

        return returnValue;
    }
    HandUtils.GetSumOfHand = GetSumOfHand;
    function HasBlackJack(hand) {
        if (hand.Cards.length == 2 && GetBestHand(hand) == 21) {
            return true;
        }
        return false;
    }
    HandUtils.HasBlackJack = HasBlackJack;
})(HandUtils || (HandUtils = {}));
//# sourceMappingURL=typescriptBlackjack.js.map
