echo %1IdleDown.anim

if exist %1IdleUp.anim copy /Y %1IdleUp.anim %2IdleUp.anim

if exist %1IdleLeft.anim copy /Y  %1IdleLeft.anim %2IdleLeft.anim

if exist %1IdleDown.anim copy /Y  %1IdleDown.anim %2IdleDown.anim

if exist %1IdleRight.anim copy /Y  %1IdleRight.anim %2IdleRight.anim


if exist %1WalkUp.anim copy /Y  %1WalkUp.anim %2WalkUp.anim

if exist %1WalkLeft.anim copy /Y  %1WalkLeft.anim %2WalkLeft.anim

if exist %1WalkDown.anim copy /Y  %1WalkDown.anim %2WalkDown.anim

if exist %1WalkRight.anim copy /Y  %1WalkRight.anim %2WalkRight.anim


#if exist %1SlashUp.anim copy /Y  %1SlashUp.anim %2SlashUp.anim
#if exist %1SlashLeft.anim copy /Y  %1SlashLeft.anim %2SlashLeft.anim

#if exist %1SlashDown.anim copy /Y  %1SlashDown.anim %2SlashDown.anim

#if exist %1SlashRight.anim copy /Y  %1SlashRight.anim %2SlashRight.anim


#if exist %1BowUp.anim copy /Y  %1BowUp.anim %2BowUp.anim

#if exist %1BowLeft.anim copy /Y  %1BowLeft.anim %2BowLeft.anim

#if exist %1BowDown.anim copy /Y  %1BowDown.anim %2BowDown.anim

#if exist %1BowRight.anim copy /Y  %1BowRight.anim %2BowRight.anim

if exist %1AttackUp.anim copy /Y  %1AttackUp.anim %2AttackUp.anim

if exist %1AttackLeft.anim copy /Y  %1AttackLeft.anim %2AttackLeft.anim

if exist %1AttackRight.anim copy /Y  %1AttackRight.anim %2AttackRight.anim

if exist %1AttackDown.anim copy /Y  %1AttackDown.anim %2AttackDown.anim


if exist %1Die.anim copy /Y  %1Die.anim %2Die.anim

if exist %1.controller copy /Y  %1.controller %2.controller