using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
public enum GameState
{
    PlayerTurn,
    ComputerTurn
}

public class GameManager : MonoBehaviour
{
    public Stack<Sprite> deck = new();
    public Stack<Sprite> player_deck = new();
    public Stack<Sprite> computer_deck = new();
    readonly Stack<Sprite> pile = new();
    Sprite player_curr_card, computer_curr_card;

    [SerializeField] Sprite[] main_deck;
    [SerializeField]
    Transform place_Point, place_pointComp;
    [SerializeField] TextMeshProUGUI infoText, turnIndicatorText;

    public bool player_chance, computer_chance;

    private GameState gameState;

    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    // Start is called before the first frame update
    void Start()
    {
        DistributeCards();
        gameState = GameState.PlayerTurn;
        turnIndicatorText.text = "Tap to play your move";
        resultPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.PlayerTurn:
                player_chance = true;
                if(player_chance && Input.touchCount > 0)
                {
                    turnIndicatorText.gameObject.SetActive(false);
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        PlayCard();
                        player_chance = false;
                        computer_chance = true;
                        gameState = GameState.ComputerTurn;
                    }
                }
                break;
            case GameState.ComputerTurn:
                if(computer_chance)
                {
                    ComputerPlay();
                    computer_chance = false;
                    player_chance = true;
                    gameState = GameState.PlayerTurn;
                }
                break;
        }
    }
    public void DistributeCards()
    {
        int deckHeight = main_deck.Length;
        int mid_len = deckHeight / 2;
        Shuffle(main_deck);
        int i = 0;
        while(i < deckHeight)
        {
            if(i < mid_len)
            {
                player_deck.Push(main_deck[i]);
            }else if(i >= mid_len)
            {
                computer_deck.Push(main_deck[i]);
            }
            i++;
        }
    }
    public void PlayCard()
    {
        AudioManager.PlayAudio("play");
        player_chance = true;
        GameObject old_card = GameObject.Find("CurrCard");
        if (old_card != null)
        {
            Destroy(old_card);
        }
        if(player_deck.Count > 0)
        {
            player_curr_card = player_deck.Pop();
            pile.Push(player_curr_card);
        }
        else
        {
            resultPanel.SetActive(true);
            resultText.text = "Computer Won!";
            infoText.text = "Computer Won!";
        }

        string currCardName = "CurrCard";
        GameObject cardGameObject = new GameObject(currCardName);
        cardGameObject.transform.position = place_Point.position;

        SpriteRenderer spriteRenderer = cardGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = player_curr_card;

        if(computer_curr_card != null && isSame(player_curr_card.name,computer_curr_card.name))
        {
            while (pile.Count != 0)
            {
                player_deck.Push(pile.Pop());
                infoText.text = "Pile pushed to player's deck";
            }
        }
    }
    public void Shuffle<T>(T[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
    public void ComputerPlay()
    {
        AudioManager.PlayAudio("play");
        GameObject old_card = GameObject.Find("CompCurrCard");
        if (old_card != null)
        {
            Destroy(old_card);
        }
        if (computer_deck.Count > 0)
        {
            computer_curr_card = computer_deck.Pop();
            pile.Push(computer_curr_card);
        }
        else
        {
            resultPanel.SetActive(true);
            resultText.text = "You Won!";
            infoText.text = "You Won!";
        }

        string currCardName = "CompCurrCard";
        GameObject cardGameObject = new GameObject(currCardName);
        cardGameObject.transform.position = place_pointComp.position;

        SpriteRenderer spriteRenderer = cardGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = computer_curr_card;

        if (player_curr_card != null && isSame(player_curr_card.name, computer_curr_card.name))
        {
            while (pile.Count != 0)
            {
                computer_deck.Push(pile.Pop());
                infoText.text = "Pile pushed to computer's deck";
            }
        }

    }
    public bool isSame(string player_card, string computer_card)
    {
        string[] playerWords = player_card.Split(' ');
        string[] computerWords = computer_card.Split(' ');

        foreach (string playerWord in playerWords)
        {
            foreach(string computerWord in computerWords)
            {
                bool wordsAreEqual = string.Equals(playerWord, computerWord, StringComparison.OrdinalIgnoreCase);
                bool isBlackOrRed = playerWord.Equals("black", StringComparison.OrdinalIgnoreCase) || playerWord.Equals("red", StringComparison.OrdinalIgnoreCase);
                bool isComputerBlackOrRed = computerWord.Equals("black", StringComparison.OrdinalIgnoreCase) || computerWord.Equals("red", StringComparison.OrdinalIgnoreCase);
                bool playerWordHasDigit = playerWord.Any(char.IsDigit);
                bool computerWordHasDigit = computerWord.Any(char.IsDigit);

                if (!isBlackOrRed && !playerWordHasDigit &&
                    !isComputerBlackOrRed && !computerWordHasDigit &&
                    wordsAreEqual)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
