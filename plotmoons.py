# -*- coding: utf-8 -*-
"""
Created on Tue Jun  6 15:31:22 2017

@author: Radu.Motrescu
"""


import matplotlib.pyplot as plt

from sklearn.datasets import make_moons
X,y=make_moons(n_samples=100,random_state=123)

plt.figure(figsize=(8,6))

plt.scatter(X[y==0, 0], X[y==0,1], color='red', alpha=0.5)
plt.scatter(X[y==1, 0], X[y==1,1], color='blue', alpha=0.5)

plt.title('A nonlinear 2Ddataset')
plt.ylabel('y coordinate')
plt.xlabel('x coordinate')

plt.show