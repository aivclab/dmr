# !/usr/bin/env python3
# -*- coding: utf-8 -*-
import pathlib
import warnings
from itertools import count

import imageio
import numpy


warnings.filterwarnings("ignore", category=UserWarning)


from tqdm import tqdm

tqdm.monitor_interval = 0

from neodroid.wrappers import CameraObservationWrapper
from contextlib import suppress

__author__ = 'cnheider'
__doc__ = ''


def generate_images(how_many=100, gamma=2.2, path=pathlib.Path.home() / 'Data' / 'drill'):
  '''

  :param path: Where to save the images
  :param how_many: How many images
  :param gamma: Gamma < 1 ~ Dark  ;  Gamma > 1 ~ Bright
  :return:
  '''

  if not path.exists():
    pathlib.Path.mkdir(path,parents=True)

  with CameraObservationWrapper(connect_to_running=True) as _environment, suppress(KeyboardInterrupt):
    for obs, frame_i in zip(tqdm(_environment, leave=False), count()):
      if how_many == frame_i:
        break

      rgb = obs['RGB']
      obj = obs['ObjectSpace']
      ocl = obs['OcclusionMask']

      name = f'stepper_{frame_i}'

      if gamma != 1:
        # with suppress(UserWarning):
        rgb = ((rgb / 255.) ** (1. / gamma))

      imageio.imwrite(str(path / f'{name}_rgb.png'), rgb)
      numpy.savez_compressed(str(path / f'{name}_obj.npz'), obj.astype(numpy.float16))
      numpy.savez_compressed(str(path / f'{name}_ocl.npz'), ocl.astype(numpy.float16))


if __name__ == '__main__':
  generate_images()
