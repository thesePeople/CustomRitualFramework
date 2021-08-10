# Creating New Rituals

This repository is for a framework for creating custom rituals and attachable effects. The rituals, outcome effects, and hediffs defined within are meant as examples. If you wish to create new rituals, all you need to do is download this mod from here and put it in your Rimworld/Mods folder. (Or alternatively, subscribe on Steam: https://steamcommunity.com/sharedfiles/filedetails/?id=2561617361 ). After that, you can create your own Mod that references the classes in this one. Make sure to load your mod after Custom Ritual Framework, and if you wish to publish it on Steam, or just want a little more ease in keeping your modlist organized, be sure to add the following to your About.xml's ModMetaData:

<modDependencies>
	<li>
		<packageId>thesepeople.RitualAttachableOutcomes</packageId>
		<displayName>Custom Ritual Framework</displayName>
	</li>
</modDependencies>
<loadAfter>
	<li>thesepeople.RitualAttachableOutcomes</li>
</loadAfter>

# Contributing to the Framework

We welcome contributors and collaborators. There are a number of ways to do this.

## Report Issues

With literally tens of thousands of mods and hundreds of thousands of potential interactions, we are bound to run into issues. If you find one, please log it in the issue tracker. The more detail the better, especially replication steps and, if applicable, a stack trace. Screenshots can also be helpful - a picture says a 1000 words!

## Resolve Issues

If you can fix anything from the issue list, feel free to make a pull request and work on it. We have no real formal review process yet, so please just be considerate. :)

##Implementing New Features

Got an idea for something to add? Great! Once more, feel free to make a pull request. There are two design philosophies that the framework tries to follow. 

1) Be as general and flexible as possible.
	
	For example, when we had an idea to implement a ritual obligation trigger on an enemy raid, we created RitualObligationTrigger_Event that has RitualObligationTrigger_EventProperties that let the user specify an eventDefName (for the triggering Incident). This way, this class can be used to create triggers for many different kinds of events.
	
2) Stay in the scope of custom rituals.

	Although the framework allows users to define rituals that add hediffs, expanding hediffs is (currently) out of scope for the project. Likewise, even other Precept mechanics are likely to be out of scope, even with new specialist roles. However, we don't want to seal away the possibility of touching on these items if there's something that requires changes to those that can b added to custom rituals.
	
## Rituals may have Obligations, but we don't: 

If you contribute something, in any way, don't feel obligated to continue contributing or supporting something you've contributed. This is a hobby project, not a career. Likewise, don't feel like you need to work on issues or anything in the 'roadmap' if you would rather implement, say, new RitualObligationTriggers or RitualStages. 