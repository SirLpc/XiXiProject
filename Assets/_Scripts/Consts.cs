

public class Consts
{

    #region Enemy fields

    public const string EnemyTag = "Enemy";

    public const string AniIsChase = "isChase";
    public const string AniTriggerAttack = "triggerAttack";
    public const string AniTriggerAttack2 = "triggerAttack2";
    public const string AniTriggerHurt = "triggerHurt";
    public const string AniTriggerDie = "triggerDie";
    public const string AniTriggerDefenseHurt = "triggerDefenseHurt";
    public const string AniTriggerNormalHurt = "triggerNormalHurt";
    public const string AniTriggerSAHurt = "triggerSAHurt";
    public const string AniIsInAttack = "isInAttack";

    public const int AniDieDuration = 2;
    public const float AniDefenseHurtDuration = .7f;

    #endregion


    #region Boss fields

    public const string BossTag = "Boss";

    public const string AniIsActive = "IsActive";
    public const string AniTriggerAtk1 = "TriggerAtk1";
    public const string AniTriggerAtk2 = "TriggerAtk2";
    public const string AniTriggerAtk3 = "TriggerAtk3";
    public const string AniBossTriggerDie = "TriggerDie";

    #endregion


    #region Player fields

    public const float AniDefenseDuration = 1f;
    public const string PlayerTag = "Player";

    #endregion


	#region Input fields

	public const string IsInputOkKey = "IsInputOkKey";
	public const string InputAttackKey = "InputAttackKey";
	public const string InputDefenseKey = "InputDefenseKey";
	public const string InputSpecialAttackKey = "InputSpecialAttackKey";

	public static string InputAttackKeyCode;
	public static string InputDefenseKeyCode;
	public static string InputSpecialAttackKeyCode;
	public const string InputHorizontal = "Horizontal";
	public const string InputVertical = "Vertical";

	#endregion

}
