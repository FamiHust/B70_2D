using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionWindowScript : WindowScript
{
    [Header("Configuration")]
    public int cardsToShow = 3;
    
    [Header("References")]
    public Transform cardsContainer;
    public GameObject teacherCardPrefab;
    
    private List<TeacherCardCtrl> _instantiatedCards = new List<TeacherCardCtrl>();

    public void Setup(TeacherCollection teacherCollection)
    {
        ClearCards();

        if (teacherCollection == null || teacherCollection.list.Count == 0)
        {
            Debug.LogWarning("TeacherCollection is empty or null!");
            return;
        }

        // Display random cards that the player hasn't collected yet
        List<TeacherData> availableData = new List<TeacherData>();
        foreach (var data in teacherCollection.list)
        {
            if (UIManager.instance != null && !UIManager.instance.playerTeachers.Contains(data))
            {
                availableData.Add(data);
            }
        }

        if (availableData.Count == 0)
        {
            Debug.Log("No more new cards available to collect!");
            // Optional: Close or show a message
            return;
        }
        
        for (int i = 0; i < cardsToShow; i++)
        {
            if (availableData.Count == 0) break;

            int randomIndex = Random.Range(0, availableData.Count);
            TeacherData selectedData = availableData[randomIndex];
            availableData.RemoveAt(randomIndex); // Don't show duplicates in the same selection

            GameObject cardObj = Instantiate(teacherCardPrefab, cardsContainer);
            TeacherCardCtrl cardCtrl = cardObj.GetComponent<TeacherCardCtrl>();
            
            if (cardCtrl != null)
            {
                cardCtrl.SetData(selectedData, OnCardSelected);
                _instantiatedCards.Add(cardCtrl);
            }
        }
    }

    private void OnCardSelected(TeacherCardCtrl selectedCard)
    {
        // Add the selected teacher to the player's inventory
        if (UIManager.instance != null)
        {
            UIManager.instance.AddTeacherToInventory(selectedCard.currentData);

            // Trigger TutorialCollection nếu đây là thẻ đầu tiên
            if (UIManager.instance.playerTeachers.Count == 1)
            {
                if (GameOverlayWindowScript.instance != null)
                {
                    GameOverlayWindowScript.instance.TriggerTutorialCollection();
                }
            }
        }

        // Close this window after selection
        Close();
    }

    private void ClearCards()
    {
        foreach (var card in _instantiatedCards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        _instantiatedCards.Clear();
    }
}
