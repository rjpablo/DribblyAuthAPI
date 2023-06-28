namespace Dribbly.Model.Enums
{
    public enum EjectionStatusEnum
    {
        NotEjected =0,
        EjectedDueNumberOfFlagrantFouls = 1,
        EjectedDueToFlagrantFoul2 = 2,
        /// <summary>
        /// Ejected due to other reason
        /// </summary>
        EjectedOther= 3
    }
}
