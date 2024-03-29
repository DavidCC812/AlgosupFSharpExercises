﻿namespace FSharpKoans
open FSharpExercises.Core
open NUnit.Framework

module ExercisesUnit5 =

    type Player = PlayerId of string

    type CardSuit =
        | Clubs     // ♣
        | Diamonds  // ♦
        | Hearts    // ♥
        | Spades    // ♠

    type CardRank =
        | Ace
        | Rank of int
        | Jack
        | Queen
        | King

    type Card = CardSuit * CardRank

    let makeCardC r s =
        let rank =
            match r with
            | 'A' -> Ace
            | '2' -> Rank 2
            | '3' -> Rank 3
            | '4' -> Rank 4
            | '5' -> Rank 5
            | '6' -> Rank 6
            | '7' -> Rank 7
            | '8' -> Rank 8
            | '9' -> Rank 9
            | 'T' -> Rank 10
            | 'J' -> Jack
            | 'Q' -> Queen
            | 'K' -> King
            | _ -> failwith "invalid rank"
        let suit =
            match s with
            | 'C' -> Clubs
            | 'D' -> Diamonds
            | 'H' -> Hearts
            | 'S' -> Spades
            | _ -> failwith "invalid suit"
        suit, rank

    let makeCard (s: string) =
        makeCardC s.[0]  s.[1]

    let makeCards cards =
        cards |> List.map makeCard

    let player1 = PlayerId "Player 1 - Alice"
    let player2 = PlayerId "Player 2 - Bob"

    [<Ignore("Not implemented");Test>]
    let BlackJackScore() =
        // Number cards count as their number, the jack, queen, and king ("face cards" or "pictures")
        // count as 10, and aces count as either 1 or 11 according to the player's choice. If the total
        // exceeds 21 points, it busts, and all bets on it immediately lose.
        // When calculating the score, always count an ace as 11 unless this would make the hand bust
        // Use an option as return type, with None meaning bust

        let calculateBlackJackScore cards =
            let scoreCard (_,rank) =
                match rank with
                | Ace -> 11
                | Rank score -> score
                | Jack
                | Queen
                | King -> 10
            let initialScore =
                List.map scoreCard cards
                |> List.sum
            if initialScore > 21 then
                let LowScoreCard (_,rank) =
                    match rank with
                    | Ace -> 1
                    | Rank score -> score
                    | Jack
                    | Queen
                    | King -> 10
                let lowScores = List.map LowScoreCard cards
                printfn "%A" lowScores
                let newScore =
                    lowScores
                    |> List.sum
                if newScore > 21 then
                    None
                else
                    Some newScore
            else
                Some initialScore

        AssertEquality (Some 13) (calculateBlackJackScore (makeCards ["3H"; "QD" ]))
        AssertEquality (Some 17) (calculateBlackJackScore (makeCards ["JD"; "5H"; "2S" ]))
        AssertEquality (Some 16) (calculateBlackJackScore (makeCards ["9C"; "3H"; "2D"; "2S" ]))
        AssertEquality (Some 21) (calculateBlackJackScore (makeCards ["AS"; "QH" ]))
        AssertEquality (Some 12) (calculateBlackJackScore (makeCards ["AH"; "3C"; "8C" ]))
        AssertEquality None (calculateBlackJackScore (makeCards ["JD"; "4S"; "8H" ]))


    [<Ignore("Not implemented");Test>]
    let PokerHands() =
        // A poker hand consists of 5 cards dealt from the deck. Poker hands are
        // ranked by the following partial order from lowest to highest.

        // - High Card: Hands which do not fit any higher category are ranked by the value
        // of their highest card. If the highest cards have the same value, the hands are
        // ranked by the next highest, and so on.
        // - Pair: 2 of the 5 cards in the hand have the same value. Hands which both contain
        // a pair are ranked by the value of the cards forming the pair. If these values are
        // the same, the hands are ranked by the values of the cards not forming the pair, in
        // decreasing order.
        // - Two Pairs: The hand contains 2 different pairs. Hands which both contain 2 pairs
        // are ranked by the value of their highest pair. Hands with the same highest pair are
        // ranked by the value of their other pair. If these values are the same the hands are
        // ranked by the value of the remaining card.
        // - Three of a Kind: Three of the cards in the hand have the same value. Hands which
        // both contain three of a kind are ranked by the value of the 3 cards.
        // - Straight: Hand contains 5 cards with consecutive values. Hands which both contain a
        // straight are ranked by their highest card.
        // - Flush: Hand contains 5 cards of the same suit. Hands which are both flushes are
        // ranked using the rules for High Card.
        // - Full House: 3 cards of the same value, with the remaining 2 cards forming a pair. Ranked
        // by the value of the 3 cards.
        // - Four of a kind: 4 cards with the same value. Ranked by the value of the 4 cards.
        // - Straight flush: 5 cards of the same suit with consecutive values. Ranked by the highest card in the hand.

        let decideHand (p1, cards1) (p2, cards2) =
            __

        AssertEquality
            (Some player1)
            (decideHand (player2, makeCards ["2H"; "3D"; "5S"; "9C"; "KD"]) (player2, makeCards ["2C"; "3H"; "4S"; "8C"; "AH"]))
        AssertEquality
            (Some player2)
            (decideHand (player2, makeCards ["2H"; "4S"; "4C"; "2D"; "4H"]) (player2, makeCards ["2S"; "8S"; "AS"; "QS"; "3S"]))
        AssertEquality
            (Some player2)
            (decideHand (player2, makeCards ["2H"; "3D"; "5S"; "9C"; "KD"]) (player2, makeCards ["2C"; "3H"; "4S"; "8C"; "KH"]))
        AssertEquality
            None
            (decideHand (player2, makeCards ["2H"; "3D"; "5S"; "9C"; "KD"]) (player2, makeCards ["2D"; "3H"; "5C"; "9S"; "KH"]))

    // by setting the sead, we ensure the same random numbers come out
    // which is useful in this testing context
    let rnd = new System.Random(42)

    let deck =
        [ for s in ['C'; 'D'; 'H'; 'S' ] do
            for r in  ['Q'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9'; 'T'; 'J'; 'Q'; 'K' ] do
                yield makeCardC r s ]

    let dealCards() =
        let toBeDelt = new ResizeArray<Card>(deck)
        let getCard() =
            let i = rnd.Next(toBeDelt.Count)
            let card = toBeDelt.[i]
            toBeDelt.RemoveAt(i)
            card

        let rec loop p1Cards p2Cards =
            if toBeDelt.Count > 1 then
                let p1Card = getCard()
                let p2Card = getCard()
                loop (p1Card :: p1Cards) (p2Card :: p2Cards)
            else
                 p1Cards, p2Cards

        loop [] []

    [<Ignore("Not implemented");Test>]
    let WarGame() =
        // There are two players.
        // The cards are all dealt equally to each player.
        // Each round, player 1 lays a card down face up at the same time that player 2 lays a card down face up. Whoever has the
        // highest value card, wins both round and take back their card, the other card is put aside.
        // The winning player's cards is added to the bottom of the winner's deck.
        // Aces are high.
        // If both cards are of equal value - both cards are discarded.
        // The player that runs out of cards loses.

        let playGame() =
            let rec playLoop plCards p2Cards =
                __

            let p1DeltCards, p2DeltCards = dealCards()

            playLoop p1DeltCards p2DeltCards

        AssertEquality player1 (playGame())
        AssertEquality player1 (playGame())
        AssertEquality player2 (playGame())