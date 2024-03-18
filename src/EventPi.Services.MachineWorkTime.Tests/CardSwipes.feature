Feature: CardSwipes

I want to be sure that card swipes produce appropriate events.


Scenario: 5 seconds swipe window test
	Given system is running
	When I swipe a card
	Then WorkOnMachineStarted event appeared
	When I waited 3 seconds
	And I swipe the card again
	Then no event should appear
	When I waited 3 seconds
	And I swipe the card again
	Then WorkOnMachineStopped event appeared
	When I waited 3 seconds
	And I swipe the card again
	Then no event should appear
	When I waited 3 seconds
	And I swipe the card again
	Then WorkOnMachineStarted event appeared
	When I waited 3 seconds
	And I swipe the card again
	Then no event should appear
	When I waited 3 seconds
	And I swipe the card again
	Then WorkOnMachineStopped event appeared
