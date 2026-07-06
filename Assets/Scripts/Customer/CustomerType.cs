namespace ScratchTicketSim.Customer
{
    /// <summary>
    /// Definiert alle möglichen Kundentypen im Shop.
    /// </summary>
    public enum CustomerType
    {
        /// <summary>Normaler Stammkunde – kauft 1 Los, mittlere Geduld.</summary>
        Regular,

        /// <summary>Gelegenheitskäufer – kauft zufällig 1–2 Lose, hohe Geduld.</summary>
        Casual,

        /// <summary>Spielsüchtiger – kauft 3–5 Lose, sehr hohe Geduld.</summary>
        Gambler,

        /// <summary>Ungeduldiger Kunde – kauft 1 Los, verlässt schnell bei langer Warteschlange.</summary>
        Impatient,

        /// <summary>VIP-Kunde – kauft teure Lose, gibt Trinkgeld, hohe Geduld.</summary>
        VIP
    }
}
