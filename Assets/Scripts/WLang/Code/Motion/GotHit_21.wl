## GotHit_21

// 扣血
//atk = @E_ATTACKER:GetAttr(ATK)
//curHp = @E_SELF:GetAttr(CUR_HP)
//hitHp = atk * Random_Range(0.5,1.5)*0.2
//@E_SELF:SetAttr(CUR_HP, curHp - hitHp)

// 位移
@E_ATTACKER:DoMove(HIT_DIR*-1.5, 2)
//idBlood = 0
//@E_SELF:LoadEffect(idBlood, MODEL_BODY, HIT_POS, HIT_DIR, 5)

//选择动画
//if(hit_is_fwd){
//    @e_self:transmotiontype(hit_fwd)
//}
//else{
//    @e_self:transmotiontype(hit_bwd)
//}

