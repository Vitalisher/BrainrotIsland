using UnityEngine;
using YG;

public class AddCoins : MonoBehaviour
{
    public string rewardID;
    public int coins;

    public void MyRewardAdvShow()
    {
        YG2.RewardedAdvShow(rewardID, () =>
        {
            PlayerStats.instance.AddCoins(coins, "coins");
        });
    }
}