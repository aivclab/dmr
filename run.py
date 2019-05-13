from itertools import count

import neodroid as neo

#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import imageio
import numpy
import warnings
warnings.filterwarnings("ignore", category=UserWarning)

__author__ = 'cnheider'
__doc__ = ''

from tqdm import tqdm

tqdm.monitor_interval = 0

from neodroid.wrappers import CameraObservationWrapper
from contextlib import suppress


def main(how_many = 10, gamma=2.2):
  '''

  :param how_many:
  :param gamma: Gamma < 1 ~ Dark  ;  Gamma > 1 ~ Bright
  :return:
  '''
  with CameraObservationWrapper(connect_to_running=True) as _environment, suppress(KeyboardInterrupt):
    for obs, frame_i in zip(tqdm(_environment, leave=False),count()):
      if how_many == frame_i:
          break

      rgb = obs['RGB']
      obj = obs['ObjectSpace']

      name = f'stepper_{frame_i}'

      if gamma != 1:
        #with suppress(UserWarning):
          rgb = ((rgb / 255.) ** (1. / gamma))

      imageio.imwrite(name + '.png',rgb)
      numpy.savez_compressed(name + '.npz', obj.astype(numpy.float16))




if __name__ == '__main__':
  main()