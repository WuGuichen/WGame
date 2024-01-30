## GotHit1

-- 选择动画
if(HIT_IS_FWD){
    @E_SELF:TransMotionType(HIT_FWD)
}
else{
    @E_SELF:TransMotionType(HIT_BWD)
}

-- 扣血
atk = @E_ATTACKER:GetAttr(ATK)
curHp = @E_SELF:GetAttr(CUR_HP)
hitHp = atk * Random_Range(0.5,1.5)*2
@E_SELF:SetAttr(CUR_HP, curHp - hitHp)

-- 位移
@E_SELF:DoMove(HIT_DIR*1.5, 2)
idBlood = 0
@E_SELF:LoadEffect(idBlood, MODEL_BODY, HIT_POS, HIT_DIR, 5)

