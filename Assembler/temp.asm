SECTION "Header", ROM0[$100]
SECTION "Game code", ROM0; This program multiplies 10 by 3
; A is only 8 bits. Max value is $FF (255)
start:
			ld a,10			; load 10 into a
			ld b,3				; load 3 into b
loop:	dec b			; decrement b (subtract 1)
			jp z, end		; if b is 0, jump to the end
			add a,10			; add 10 to a
			jp loop			; jump to the top of the multiply loop
end:		halt					; end. a should be equal to 30

halt