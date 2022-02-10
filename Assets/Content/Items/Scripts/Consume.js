function CanStack() { return true; }

function Stack(original, toStack) {
    original.Count += toStack.Count;
}

function CanConsume() {
    return true;
}

function Consume(heal) {
    Player.Instance.Damage(-heal);
}