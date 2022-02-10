const Clr = importNamespace("");

function CanStack() { return true; }

function Stack(original, toStack) {
    original.Count += toStack.Count;
}

function CanUse(_, type) {
    if(type !== undefined) {
        if(!Clr.Player.Instance.IsEquippingSpecificWeapon(type))
            return false;
    }
    return Clr.Player.Instance.IsEquippingWeapon;
}

function Use(ammo, type) {
    Clr.Player.Instance.CurrentWeapon.AllAmmo += ammo;
    Clr.Player.Instance.UpdateWeaponUI(Clr.Player.Instance.CurrentWeapon);
    Clr.Player.Instance.CurrentWeapon.UpdateItemData();
}