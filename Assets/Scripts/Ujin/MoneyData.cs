public class MoneyData
{
    public int Money { get; private set; }

    public MoneyData(int initial = 0)
    {
        Money = initial;
    }

    // 돈 추가하기
    public void Add(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
    }
    
    // 돈 삭제하기
    public void Delete(int amount)
    {
        Money -= amount;

        if(Money <= 0) Money = 0;
    }
}