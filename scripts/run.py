# !/usr/bin/env python3
# -*- coding: utf-8 -*-
import pathlib
import warnings
from itertools import count

import numpy

warnings.filterwarnings("ignore", category=UserWarning)

from tqdm import tqdm
import matplotlib.pyplot as plt

tqdm.monitor_interval = 0
import numpy
from neodroid.wrappers import CameraObservationWrapper
from contextlib import suppress

__author__ = 'Christian Heider Nielsen'
__doc__ = ''


def generate_images(how_many=1, gamma=2.2, path=pathlib.Path.home() / 'Data' / 'drill'):
  '''

  :param path: Where to save the images
  :param how_many: How many images
  :param gamma: Gamma < 1 ~ Dark  ;  Gamma > 1 ~ Bright
  :return:
  '''

  if not path.exists():
    pathlib.Path.mkdir(path, parents=True)

  with CameraObservationWrapper(connect_to_running=True) as _environment, suppress(KeyboardInterrupt):
    for obs, frame_i in zip(tqdm(_environment, leave=False), count()):
      if how_many == frame_i:
        break

      rgb = obs['RGB']
      shape = 128
      rgb = rgb.reshape((shape, shape, 4))

      # rgb = (rgb.astype(numpy.float32) / 255.) ** (1. / 2.2)
      pos = obs['ObjectSpace']
      pos = pos.reshape((shape, shape, 4)).copy()
      pos[:, :, :-1] -= 0.5
      ocl = obs['OcclusionMask']
      ocl = ocl.reshape((shape, shape, 4))
      # ocl = ocl[:, :, -1].astype(numpy.uint8) // 255

      print(numpy.min(rgb), numpy.max(rgb))
      print(numpy.max(pos))
      print(numpy.max(ocl))

      for i in range(3):
        pos[:, :, i] -= numpy.min(pos[:, :, i])
        pos[:, :, i] /= numpy.max(pos[:, :, i])

      plt.imshow(pos[:, :, :3].astype(numpy.float32))
      plt.show()
      plt.imshow(rgb[:, :, :3].astype(numpy.float32))
      plt.show()

      name = f'stepper_{frame_i}'

      # imageio.imwrite(str(path / f'{name}_rgb.png'), rgb)
      numpy.savez_compressed(str(path / f'{name}_rgb.npz'), rgb)
      numpy.savez_compressed(str(path / f'{name}_obj.npz'), pos)
      numpy.savez_compressed(str(path / f'{name}_ocl.npz'), ocl)


if __name__ == '__main__':
  generate_images()
