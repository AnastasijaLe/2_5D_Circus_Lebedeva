using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject entryPrefab;          
    public Transform contentParent;          

    private void OnEnable()
    {
        PopulateRanking();
    }

    public void PopulateRanking()
    {
        
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        
        SaveLoadScript save = FindObjectOfType<SaveLoadScript>();
        if (save == null)
        {
            Debug.LogError("SaveLoadScript not found!");
            return;
        }

       List<PlayerResult> rankingList = save.GetRankingList();

        
        rankingList.Sort((a, b) => b.points.CompareTo(a.points));

        
        foreach (var result in rankingList)
        {
            GameObject entry = Instantiate(entryPrefab, contentParent);
            RectTransform rt = entry.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 80); // вручную задаём высоту и ширину
            entry.GetComponent<Image>().color = Color.red; // фон
            entry.transform.Find("NameText").GetComponent<TMPro.TextMeshProUGUI>().color = Color.black;
            entry.transform.Find("PointsText").GetComponent<TMPro.TextMeshProUGUI>().color = Color.black;
            entry.transform.Find("TimeText").GetComponent<TMPro.TextMeshProUGUI>().color = Color.black;

LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
            
            entry.transform.Find("NameText").GetComponent<TMP_Text>().text = result.playerName;
            entry.transform.Find("PointsText").GetComponent<TMP_Text>().text = result.points.ToString() + " P";
            entry.transform.Find("TimeText").GetComponent<TMP_Text>().text = result.finishTime;

            Sprite avatarSprite = Resources.Load<Sprite>("Avatars/" + result.avatarSpriteName);
            if (avatarSprite != null)
                entry.transform.Find("Avatar").GetComponent<Image>().sprite = avatarSprite;
        }
    }
}

