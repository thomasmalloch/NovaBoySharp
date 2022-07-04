;---------------------------------------;
; This program multiplies 56 by 60 ;
;---------------------------------------;
; HL is the 16bit accumulator, so it's
; capable of computer larger values
;---------------------------------------
start:
			; the 16bit add only supports the CPU registers,
			; so we need to use DE instead of a constant
			ld hl,56			; load 56 into HL
			ld de,56			; load 56 into DE
			ld b,60			; load 60 into b
loop:	dec b				; decrement b (subtract 1)
			jp z, end		; if b is 0, jump to the end
			add hl,de		; add DE to HL
			jp loop			; jump to the top of the multiply loop
end:		halt					; end. HL should be 3360
