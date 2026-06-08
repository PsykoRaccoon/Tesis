public interface IAbility
{
    bool IsActive { get; set; }
    bool IsUsingAbility();
    void LockVisuals();  
    void UnlockVisuals();
}