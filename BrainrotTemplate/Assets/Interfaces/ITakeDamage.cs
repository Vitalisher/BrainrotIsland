public interface ITakeDamage
{
    public void TakeDamage(int damage);
    public void Die();
    
    public bool isDead { get; set; }
}
