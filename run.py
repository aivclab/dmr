import neodroid as neo


def rollout(env):
    pass


if __name__ == '__main__':

    env = neo.make("dmr")
    while True:
        episode_reward = rollout(env)
        env.reset()

    env.close()