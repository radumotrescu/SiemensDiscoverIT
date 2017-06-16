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
    print(X)
    mat_sq_dists=squareform(sq_dist)
    #print(mat_sq_dists)
    K=exp(-gamma * mat_sq_dists)
    
    N=K.shape[0]
    one_n=np.ones((N,N))/N
    K=K-one_n.dot(K) - K.dot(one_n) + one_n.dot(K).dot(one_n)
    print('K')
    print(K)
    eigvals, eigvecs=eigh(K)
    
    np.savetxt('eigvectorsunsorted.txt',eigvecs,fmt='%.5f')
    np.savetxt('eigvaluesunsorted.txt',eigvals)
    
    #print('alphas')
   
    alphas = np.column_stack((eigvecs[:,-i] for i in range(1,n_components+1)))
    lambdas = [eigvals[-i] for i in range(1,n_components+1)]
    #print(alphas)
    #print('lambdas')
    #print(lambdas)
    return mat_sq_dists,one_n,K,alphas, lambdas


from sklearn.datasets import make_moons
from sklearn.decomposition import PCA

cloud1=np.loadtxt('cloud1.txt')
t = np.reshape(cloud1,(-1,2))
print(t)

cloud2=np.loadtxt('cloud2.txt')
z=np.reshape(cloud2,(-1,2))
print(z)
X1, y = make_moons(n_samples=100, random_state=123)
X=np.vstack((t,z))
print(X)



mat_sq_dists,one_n,K,alphas, lambdas = stepwise_kpca(X, gamma=15, n_components=100)

np.savetxt('kerneldata.txt',alphas[:,[0,1]], fmt='%.5f')

n=alphas*lambdas
m=lambdas*alphas
c=n-m


np.savetxt('eigvectorssorted.txt',alphas,fmt='%.5f')
np.savetxt('eigvaluessorted.txt',lambdas)

x_new = X[0]
X_proj = alphas[0]
#print(x_new)

def project_x(x_new, X, gamma, alphas, lambdas):
    print('alphas')
    print(alphas)
    print('lambdas')
    print(lambdas)
    
    print('x_new')
    print(x_new)
    print('X')
    print(X)
    
    #for row in X:
        #print(((x_new-row)**2))
    pair_dist = np.array([np.sum((x_new-row)**2) for row in X])
    print('pair-dist')
    print(pair_dist)

    k = np.exp(-gamma * pair_dist)
    print('k')
    print(k)
    x=(alphas/lambdas)
    print('x')
    print(x)
    return x,k,k.dot(x)

# projection of the "new" datapoint

x,k,x_reproj = project_x(x_new, X, gamma=15, alphas=alphas, lambdas=lambdas)
print("xreproj")
print(x_reproj)

extractedData=alphas[:,[0,1]]

scikit_pca=PCA(n_components=2)
X_spca=scikit_pca.fit_transform(X)
a=scikit_pca.components_
ax=scikit_pca.explained_variance_

testPCA=PCA(n_components=2)
extractedDataPCA=testPCA.fit_transform(extractedData)

bx=testPCA.explained_variance_
b=testPCA.components_

plt.figure(figsize=(8,6))
plt.scatter(alphas[y==0, 0], np.zeros((50)), color='red', alpha=0.5)
plt.scatter(alphas[y==1, 0], np.zeros((50)), color='blue', alpha=0.5)
plt.scatter(X_proj, 0, color='black', label='original projection of point X[24]', marker='^', s=100)
plt.scatter(x_reproj, 0, color='green', label='remapped point X[24]', marker='x', s=500)
plt.legend(scatterpoints=1)
plt.show()


#from sklearn.datasets import make_moons
#X,y=make_moons(n_samples=90,random_state=123)

#print(len(y))
#
#cloud1=np.loadtxt('cloud1initial.txt')
#cloud2=np.loadtxt('cloud2initial.txt')
#
#zeroes=np.zeros(cloud1.shape[0])
#print (len(zeroes))
#
#ones=np.ones(cloud2.shape[0])
#print (len(ones))
#
#y=np.concatenate((zeroes,ones))
#
#print(len(y))
#
#print(cloud1)
#print('\n')
#print(cloud2)
#
#plt.figure(figsize=(8,6))
#
#totalpoints=np.vstack((cloud1,cloud2))
#print('totalpoints')
#print(totalpoints)
#
##pointsA=(X[y==0,0],X[y==0,1])
##pointsB=(X[y==1,0],X[y==1,1])
#
##matrixA=np.column_stack((pointsA[0], pointsA[1]))
##matrixB=np.column_stack((pointsB[0], pointsB[1]))
#
#
##print('\n')
##print(pointsA)
##print('\n')
##print(pointsB)
##print('\n')
#plt.title('A nonlinear 2Ddataset')
#plt.ylabel('y coordinate')
#plt.xlabel('x coordinate')
#X_pc=stepwise_kpca(totalpoints,15,2)
#print('xpc')
#print(X_pc)
#
#plt.scatter(X_pc[y==0, 0], X_pc[y==0,1], color='red', alpha=0.5)
#plt.scatter(X_pc[y==1, 0], X_pc[y==1,1], color='blue', alpha=0.5)
#
#plt.show

#np.savetxt('outfile.txt',X_pc, fmt=' %.5f', delimiter='\n')
#np.savetxt('data.txt', X, fmt=' %.5f', delimiter='\n')
#np.savetxt('redpoints.txt', matrixA, fmt=' %.5f', delimiter='\n')
#np.savetxt('bluepoints.txt', matrixB, fmt=' %.5f', delimiter='\n')









