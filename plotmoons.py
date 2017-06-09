# -*- coding: utf-8 -*-
"""
Created on Tue Jun  6 15:31:22 2017

@author: Radu.Motrescu
"""


import matplotlib.pyplot as plt
from scipy.spatial.distance import pdist, squareform
from scipy import exp
from scipy.linalg import eigh
import numpy as np


def stepwise_kpca(X, gamma, n_components):
    sq_dist=pdist(X, 'sqeuclidean')
    
    mat_sq_dists=squareform(sq_dist)
    
    K=exp(-gamma * mat_sq_dists)
    
    N=K.shape[0]
    one_n=np.ones((N,N))/N
    K=K-one_n.dot(K) - K.dot(one_n) + one_n.dot(K).dot(one_n)
    
    eigvals, eigvecs=eigh(K)
    
    
    X_pc=np.column_stack((eigvecs[:,-i] for i in range (1,n_components+1)))
    print (eigvals)
    return X_pc



from sklearn.datasets import make_moons
X,y=make_moons(n_samples=100,random_state=123)

plt.figure(figsize=(8,6))

pointsA=(X[y==0,0],X[y==0,1])
pointsB=(X[y==1,0],X[y==1,1])

matrixA=np.column_stack((pointsA[0], pointsA[1]))
matrixB=np.column_stack((pointsB[0], pointsB[1]))


print('\n')
print(pointsA)
print('\n')
print(pointsB)
print('\n')
plt.title('A nonlinear 2Ddataset')
plt.ylabel('y coordinate')
plt.xlabel('x coordinate')
X_pc=stepwise_kpca(X,15,2)
print(X)

plt.scatter(X_pc[y==0, 0], X_pc[y==0,1], color='red', alpha=0.5)
plt.scatter(X_pc[y==1, 0], X_pc[y==1,1], color='blue', alpha=0.5)

np.savetxt('outfile.txt',X_pc, fmt=' %.5f', delimiter='\n')
np.savetxt('data.txt', X, fmt=' %.5f', delimiter='\n')
np.savetxt('redpoints.txt', matrixA, fmt=' %.5f', delimiter='\n')
np.savetxt('bluepoints.txt', matrixB, fmt=' %.5f', delimiter='\n')


plt.show


