
# Destroy Comets

![Environment](images/DestroyComets/environment.png)

## Model Details

### Environment

Destroy Comets features a 2D environment where a cannon has to shoot comets while not getting hit. Comets spawn on a certain radius and fly towards the comet with altering displacements on their velocity. The cannon has a little shooting cooldown as constraint.

### Output Actions

The action space is continuous. The agent is able to rotate and shoot. Shooting is actually a discrete action, but is treated as continuous for now.

### State Input

The agent has a total number of 18 inputs. It has 16 "eyes" (like seen below) to measure the distance to comets. The vision rotates along with the cannon. The remaining information of the state space are the current rotation of the agent and the shooting cooldown.

![Input](images/DestroyComets/eyes.png)

### Reward Signals

The agent is rewarded for shooting a comet (+1) and punished for getting hit by one (-1).

## Results

One result can be watched [here](https://www.youtube.com/watch?v=d1HEhY7TwSE). If shooting is not constrained, the agent learns to keep rotating and shooting. A proper result with the cooldown is not achieved yet. A potential solution is to treat the action space completely discrete. In general, having discrete and continuous actions is a future challenge.