namespace Lazy
{
    /// <summary>
    /// Interface implementing lazy calculation.
    /// </summary>
    /// <typeparam name="T"> Type of calculated value.</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Calculates value.
        /// </summary>
        /// <returns> Value that has been calculated. </returns>
        T Get();
    }
}
