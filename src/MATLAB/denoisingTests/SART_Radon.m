

s=size(sinoGram);
nProjections = s(2);

angleStep=360.0/nProjections;

sizeProjections = s(1);

jumpSize =floor( nProjections/4);

reconImageSART=reconImage;

R_Thresh = (sizeProjections/2.1)^2;
centerI= sizeProjections/2;
centerJ= sizeProjections/2;
mask=zeros([sizeProjections sizeProjections]);

for I=1:sizeProjections
        for J=1:sizeProjections
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1
                mask(I,J)=1;
            end
        end
end
maskIndex = find(mask==1);

angleI=1;
for K=1:5  
    R=randperm(length(thetas));
    for I=1:nProjections
        
        reconImageSART(maskIndex)=0;
        angleI = R(I);
        angle=thetas( angleI);
        
        projection = sinoGram(:,angleI);

        forward=radon(reconImageSART ,angle);
        startI=round((length(forward)-length(projection))/2);
        t=forward(startI:end-startI);
        
        diff(:,1)=(projection -t)/sizeProjections;
        diff(:,2)= diff(:,1);
   
        
        addition=iradon(diff,[angle angle],'linear','none',1,s(1));
        
        imagesc(addition);drawnow;
        
        reconImageSART=reconImageSART + 1.0/K* addition;
    end
end

figure;imagesc(reconImageSART(70:end-70,70:end-70));colormap gray;
