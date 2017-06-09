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



#from sklearn.datasets import make_moons
#X,y=make_moons(n_samples=90,random_state=123)

#print(len(y))

cloud1=np.loadtxt('cloud1initial.txt')
cloud2=np.loadtxt('cloud2initial.txt')

zeroes=np.zeros(cloud1.shape[0])
print (len(zeroes))

ones=np.ones(cloud2.shape[0])
print (len(ones))

y=np.concatenate((zeroes,ones))

print(len(y))

print(cloud1)
print('\n')
print(cloud2)

plt.figure(figsize=(8,6))

totalpoints=np.vstack((cloud1,cloud2))
print('totalpoints')
print(totalpoints)

#pointsA=(X[y==0,0],X[y==0,1])
#pointsB=(X[y==1,0],X[y==1,1])

#matrixA=np.column_stack((pointsA[0], pointsA[1]))
#matrixB=np.column_stack((pointsB[0], pointsB[1]))


#print('\n')
#print(pointsA)
#print('\n')
#print(pointsB)
#print('\n')
plt.title('A nonlinear 2Ddataset')
plt.ylabel('y coordinate')
plt.xlabel('x coordinate')
X_pc=stepwise_kpca(totalpoints,15,2)
print('xpc')
print(X_pc)

plt.scatter(X_pc[y==0, 0], X_pc[y==0,1], color='red', alpha=0.5)
plt.scatter(X_pc[y==1, 0], X_pc[y==1,1], color='blue', alpha=0.5)

plt.show

555
#np.savetxt('outfile.txt',X_pc, fmt=' %.5f', delimiter='\n')
#np.savetxt('data.txt', X, fmt=' %.5f', delimiter='\n')
#np.savetxt('redpoints.txt', matrixA, fmt=' %.5f', delimiter='\n')
#np.savetxt('bluepoints.txt', matrixB, fmt=' %.5f', delimiter='\n')





